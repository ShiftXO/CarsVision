namespace CarsVision.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using AngleSharp;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Models;

    public class CarsScrapperService : ICarsScrapperService
    {
        private static readonly string EmptyString = string.Empty;

        private readonly IConfiguration config;
        private readonly IBrowsingContext context;
        private readonly IDeletableEntityRepository<Car> carsRepository;
        private readonly IDeletableEntityRepository<Make> makesRepository;
        private readonly IDeletableEntityRepository<Model> modelsRepository;
        private readonly IDeletableEntityRepository<Extra> extrasRepository;
        private readonly IRepository<Color> colorRepository;

        public CarsScrapperService(
            IDeletableEntityRepository<Car> carsRepository,
            IDeletableEntityRepository<Make> makesRepository,
            IDeletableEntityRepository<Model> modelsRepository,
            IDeletableEntityRepository<Extra> extrasRepository,
            IRepository<Color> colorRepository)
        {
            this.config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(this.config);

            this.carsRepository = carsRepository;
            this.makesRepository = makesRepository;
            this.modelsRepository = modelsRepository;
            this.extrasRepository = extrasRepository;
            this.colorRepository = colorRepository;
        }

        public async void PopulateDb(int pagesCount)
        {
            var bag = new ConcurrentBag<CarDto>();

            Parallel.For(1, 2, (i) =>
            {
                var cars = this.GetCar(i);
                Parallel.ForEach(cars, (car) =>
                {
                    bag.Add(car);
                });
            });

            await this.AddCarToDbAsync(bag);
            await this.carsRepository.SaveChangesAsync();
        }

        private async Task AddCarToDbAsync(ConcurrentBag<CarDto> cars)
        {
            foreach (var car in cars)
            {
                var makeId = await this.GetOrCreateMakeAsync(car.Make);
                var modelId = await this.GetOrCreateModelAsync(makeId, car.Model);
                var colorId = await this.GetOrCreateColorAsync(car.ColorName);

                await this.CreateExtrasAsync(car.ExtraNames);

                var newCar = new Car
                {
                    MakeId = makeId,
                    ModelId = modelId,
                    Modification = car.Modification,
                    ImageUrl = car.ImageUrl,
                    Mileage = car.Mileage,
                    Power = car.Power,
                    ColorId = colorId,
                    Price = car.Price,
                    Location = car.Location,
                    Year = car.Year,
                    Description = car.Description,
                    Currency = (Currency)Enum.Parse(typeof(Currency), car.Currency == "лв." ? "BGN" : car.Currency, true),
                    EngineType = (EngineType)Enum.Parse(typeof(EngineType), car.EngineType, true),
                    Gearbox = (Gearbox)Enum.Parse(typeof(Gearbox), car.Gearbox, true),
                    EuroStandard = (EuroStandard)Enum.Parse(typeof(EuroStandard), car.EuroStandard, true),
                };

                var extrasList = await this.GetExtrasAsync(car.ExtraNames, newCar.Id);

                newCar.Extras = extrasList;

                await this.carsRepository.AddAsync(newCar);
            }

            await this.carsRepository.SaveChangesAsync();

            ;
        }

        private ConcurrentBag<CarDto> GetCar(int id)
        {
            var bag = new ConcurrentBag<CarDto>();

            var page = this.context
                .OpenAsync($"https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=htp0dq&f1={id}")
                .GetAwaiter().GetResult();

            for (int i = 23; i <= 42; i++)
            {
                var carUrlQuery = page.QuerySelectorAll($"#mainholder > table:nth-child(4) > tbody > tr:nth-child(1) > td:nth-child(1) > form:nth-child(4) > table:nth-child({i}) > tbody > tr:nth-child(2) > td:nth-child(2) > a");

                var carUrlValue = carUrlQuery[0].OuterHtml.Split("class")[0].ToString();

                carUrlValue = carUrlValue.Split("<a href=", StringSplitOptions.RemoveEmptyEntries)[0];

                var carUrl = carUrlValue.Replace("\"", EmptyString).ToString();

                carUrl = carUrl.Replace("\"", EmptyString);
                carUrl = "https:" + carUrl;

                var car = this.GetData(carUrl);
                bag.Add(car);
            }

            return bag;
        }

        private CarDto GetData(string url)
        {
            var document = this.context.OpenAsync(url).GetAwaiter().GetResult();

            var carDataQuery = document.QuerySelector(".dilarData");
            var carDataString = carDataQuery.TextContent.Trim();
            var carDataArr = carDataString
                .Replace("Дата на производство", string.Empty)
                .Replace("Тип двигател", EmptyString)
                .Replace("Мощност", EmptyString)
                .Replace("Евростандарт", EmptyString)
                .Replace("Скоростна кутия", EmptyString)
                .Replace("Категория", EmptyString)
                .Replace("Пробег", EmptyString)
                .Replace("Цвят", EmptyString)
                .Split("\n");

            var carProductionMonth = EmptyString;
            var carProductionYear = EmptyString;
            var carEngineType = EmptyString;
            var carPower = EmptyString;
            var carEuroStandard = EmptyString;
            var carGearbox = EmptyString;
            var carCategory = EmptyString;
            var carMileage = EmptyString;
            var carColor = EmptyString;

            if (!carDataString.Contains("Пробег"))
            {
                throw new InvalidOperationException();
            }

            carProductionMonth = carDataArr[0].Split()[0];
            carProductionYear = carDataArr[0].Split()[1];

            if (carDataString.Contains("Мощ") && carDataString.Contains("Евро") && carDataString.Contains("Цвят"))
            {
                carEngineType = carDataArr[1];
                carPower = carDataArr[2];
                carEuroStandard = carDataArr[3];
                carGearbox = carDataArr[4];
                carCategory = carDataArr[5];
                carMileage = carDataArr[6];
                carColor = carDataArr[7];
            }
            else if (carDataString.Contains("Мощ") && carDataString.Contains("Евро"))
            {
                carEngineType = carDataArr[1];
                carPower = carDataArr[2];
                carEuroStandard = carDataArr[3];
                carGearbox = carDataArr[4];
                carCategory = carDataArr[5];
                carMileage = carDataArr[6];
            }
            else if (carDataString.Contains("Мощ") && carDataString.Contains("Цвят"))
            {
                carEngineType = carDataArr[1];
                carPower = carDataArr[2];
                carGearbox = carDataArr[3];
                carCategory = carDataArr[4];
                carMileage = carDataArr[5];
                carColor = carDataArr[6];
            }
            else if (carDataString.Contains("Евро") && carDataString.Contains("Цвят"))
            {
                carEngineType = carDataArr[1];
                carEuroStandard = carDataArr[2];
                carGearbox = carDataArr[3];
                carCategory = carDataArr[4];
                carMileage = carDataArr[5];
                carColor = carDataArr[6];
            }
            else if (carDataString.Contains("Мощ") && !carDataString.Contains("Евро") && !carDataString.Contains("Цвят"))
            {
                carEngineType = carDataArr[1];
                carPower = carDataArr[2];
                carGearbox = carDataArr[3];
                carCategory = carDataArr[4];
                carMileage = carDataArr[5];
            }
            else if (carDataString.Contains("Евро") && !carDataString.Contains("Мощ") && !carDataString.Contains("Цвят"))
            {
                carEngineType = carDataArr[1];
                carEuroStandard = carDataArr[2];
                carGearbox = carDataArr[3];
                carCategory = carDataArr[4];
                carMileage = carDataArr[5];
            }
            else if (carDataString.Contains("Цвят") && !carDataString.Contains("Мощ") && !carDataString.Contains("Евро"))
            {
                carEngineType = carDataArr[1];
                carGearbox = carDataArr[2];
                carCategory = carDataArr[3];
                carMileage = carDataArr[4];
                carColor = carDataArr[5];
            }
            else
            {
                carEngineType = carDataArr[1];
                carGearbox = carDataArr[2];
                carCategory = carDataArr[3];
                carMileage = carDataArr[4];
            }

            if (carEngineType == "Дизелов")
            {
                carEngineType = "Diesel";
            }
            else if (carEngineType == "Бензинов")
            {
                carEngineType = "Gasoline";
            }
            else if (carEngineType == "Електрически")
            {
                carEngineType = "Electric";
            }
            else
            {
                carEngineType = "Hybrid";
            }

            if (carGearbox == "Ръчна")
            {
                carGearbox = "Manual";
            }
            else if (carGearbox == "Автоматична")
            {
                carGearbox = "Automatic";
            }
            else if (carGearbox == "Полуавтоматична")
            {
                carGearbox = "Semi_Automatic";
            }

            carEuroStandard = carEuroStandard.Replace("Евро ", "Euro_");

            carPower = carPower.Replace("к.с.", EmptyString);
            carMileage = carMileage.Replace("км", EmptyString);

            var carNameQuery = document.QuerySelector("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(1) > form:nth-child(3) > div:nth-child(16) > table > tbody > tr > td:nth-child(1) > div:nth-child(1) > h1");
            var priceQuery = document.QuerySelector("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(1) > form:nth-child(3) > div:nth-child(16) > table > tbody > tr > td:nth-child(2) > div > strong");

            var locationQuery = document.QuerySelector("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(2) > div:nth-child(1) > div > div > div:nth-child(5)");
            if (locationQuery == null)
            {
                if (document.QuerySelectorAll("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(2) > div:nth-child(1) > div > div > div.adress").Length == 1)
                {
                    locationQuery = document.QuerySelectorAll("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(2) > div:nth-child(1) > div > div > div.adress")[0];
                }
                else
                {
                    locationQuery = document.QuerySelectorAll("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(2) > div:nth-child(1) > div > div > div.adress")[1];
                }
            }

            var carLocation = locationQuery.TextContent;

            var carDescription = string.Empty;
            var carExtras = string.Empty;

            for (int i = 1; i < 50; i++)
            {
                var descriptionQuery = document.QuerySelector($"#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(1) > form:nth-child(3) > div:nth-child({i})");

                if (descriptionQuery != null)
                {
                    var extrasQuery = document.QuerySelector("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(1) > form:nth-child(3) > table:nth-child(21) > tbody > tr > td:nth-child(1)");
                    var extrasQuery2 = document.QuerySelector("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(1) > form:nth-child(3) > table:nth-child(21) > tbody > tr > td:nth-child(2)");
                    var extrasQuery3 = document.QuerySelector("#mainholder > table:nth-child(4) > tbody > tr > td:nth-child(1) > form:nth-child(3) > table:nth-child(21) > tbody > tr > td:nth-child(3)");

                    if (extrasQuery != null && extrasQuery2 != null && extrasQuery3 != null && carExtras == EmptyString)
                    {
                        carExtras += extrasQuery.TextContent;
                        carExtras += extrasQuery2.TextContent;
                        carExtras += extrasQuery3.TextContent;
                    }

                    if (descriptionQuery.TextContent.Contains("Допълнителна информация:"))
                    {
                        carDescription = descriptionQuery.NextElementSibling.TextContent.Trim();
                    }
                }
            }

            var imageQuery = document.QuerySelector("#bigPictureCarousel");
            var carImageUrl = imageQuery.OuterHtml.Split("\"")[1];

            var dataArr = carNameQuery.TextContent.Split(" ", 3);
            var carMake = dataArr[0];
            var carModel = dataArr[1];
            var carModification = string.Empty;

            if (dataArr.Length > 2)
            {
                carModification += dataArr[2];
            }

            var carPrice = string.Empty;
            var carCurrency = string.Empty;

            var priceData = priceQuery.TextContent.Split();

            if (priceData[0] == "По")
            {
                carPrice = "По договаряне";
                carCurrency = null;
            }
            else if (priceData[0].Length == 2)
            {
                var priceArr = priceQuery.TextContent.Split(" ");
                carPrice = priceArr[0] + priceArr[1];
                carCurrency = priceArr[2];
            }
            else if (priceData[0].Length == 1)
            {
                carPrice = priceData[0] + priceData[1];
                carCurrency = priceData[2];
            }

            var extrasArr = carExtras.Split("\n");
            var carExtrasList = new List<string>();

            for (int i = 0; i < extrasArr.Length; i++)
            {
                if (extrasArr[i].Contains("Безопасност") ||
                    extrasArr[i].Contains("Екстериор") ||
                    extrasArr[i].Contains("Комфорт") ||
                    extrasArr[i].Contains("Защита") ||
                    extrasArr[i].Contains("Интериор") ||
                    extrasArr[i].Contains("Други"))
                {
                    continue;
                }

                carExtrasList.Add(extrasArr[i].Replace("\n", EmptyString).Replace("•", EmptyString));
            }

            var car = new CarDto
            {
                Make = carMake,
                Model = carModel,
                Modification = carModification,
                Price = decimal.Parse(carPrice),
                ImageUrl = carImageUrl,
                EngineType = carEngineType,
                Gearbox = carGearbox,
                Power = int.Parse(carPower),
                Year = string.Concat(carProductionMonth, " ", carProductionYear),
                Mileage = int.Parse(carMileage),
                ColorName = carColor,
                EuroStandard = carEuroStandard,
                Currency = carCurrency,
                Location = carLocation,
                Description = carDescription,
                ExtraNames = carExtrasList,
            };

            return car;
        }

        private async Task<int> GetOrCreateMakeAsync(string carMake)
        {
            var make = this.makesRepository
                .AllAsNoTracking()
                .FirstOrDefault(x => x.Name == carMake);

            if (make != null)
            {
                return make.Id;
            }

            make = new Make
            {
                Name = carMake,
            };

            await this.makesRepository.AddAsync(make);
            await this.makesRepository.SaveChangesAsync();

            return make.Id;
        }

        private async Task<int> GetOrCreateModelAsync(int carMakeId, string modelName)
        {
            var model = this.modelsRepository
                .AllAsNoTracking().FirstOrDefault(x => x.MakeId == carMakeId);

            if (model != null)
            {
                return model.Id;
            }

            model = new Model
            {
                MakeId = carMakeId,
                Name = modelName,
            };

            await this.modelsRepository.AddAsync(model);
            await this.modelsRepository.SaveChangesAsync();

            return model.Id;
        }

        private async Task<int> GetOrCreateColorAsync(string colorName)
        {
            var color = this.colorRepository
                .AllAsNoTracking().FirstOrDefault(x => x.Name == colorName);

            if (color != null)
            {
                return color.Id;
            }

            color = new Color
            {
                Name = colorName,
            };

            await this.colorRepository.AddAsync(color);
            await this.colorRepository.SaveChangesAsync();

            return color.Id;
        }

        private async Task CreateExtrasAsync(ICollection<string> extras)
        {
            foreach (var extraName in extras)
            {
                var extra = this.extrasRepository
                    .AllAsNoTracking().FirstOrDefault(x => x.Name == extraName);

                if (extra != null)
                {
                    continue;
                }

                extra = new Extra
                {
                    Name = extraName,
                };

                await this.extrasRepository.AddAsync(extra);
                await this.extrasRepository.SaveChangesAsync();
            }
        }

        private async Task<List<CarsExtras>> GetExtrasAsync(ICollection<string> extras, int carId)
        {
            var extrasToReturn = new List<CarsExtras>();

            foreach (var extraName in extras)
            {
                var extra = this.extrasRepository
                    .AllAsNoTracking().FirstOrDefault(x => x.Name == extraName);

                var carExtra = new CarsExtras
                {
                    ExtraId = extra.Id,
                    CarId = carId,
                };

                extrasToReturn.Add(carExtra);
            }

            return extrasToReturn;
        }
    }
}
