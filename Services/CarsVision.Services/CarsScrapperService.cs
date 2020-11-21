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

        private readonly IBrowsingContext context;
        private readonly IDeletableEntityRepository<Car> carsRepository;
        private readonly IDeletableEntityRepository<Make> makesRepository;
        private readonly IDeletableEntityRepository<Model> modelsRepository;
        private readonly IDeletableEntityRepository<Extra> extrasRepository;
        private readonly IRepository<CarsExtras> carsExtrasRepository;
        private readonly IRepository<Color> colorRepository;

        public CarsScrapperService(
            IDeletableEntityRepository<Car> carsRepository,
            IDeletableEntityRepository<Make> makesRepository,
            IDeletableEntityRepository<Model> modelsRepository,
            IDeletableEntityRepository<Extra> extrasRepository,
            IRepository<CarsExtras> carsExtrasRepository,
            IRepository<Color> colorRepository)
        {
            this.carsRepository = carsRepository;
            this.makesRepository = makesRepository;
            this.modelsRepository = modelsRepository;
            this.extrasRepository = extrasRepository;
            this.colorRepository = colorRepository;
            this.carsExtrasRepository = carsExtrasRepository;

            var config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(config);
        }

        public async void PopulateDb(int pagesCount)
        {
            var bag = new ConcurrentBag<CarDto>();

            Parallel.For(1, 2, (i) =>
            {
                var cars = this.GetCars(i).GetAwaiter().GetResult();
                foreach (var car in cars)
                {
                    bag.Add(car);
                }
            });

            foreach (var car in bag)
            {
                if (car == null)
                {
                    continue;
                }

                var makeId = this.GetOrCreateMakeAsync(car.Make.Trim()).GetAwaiter().GetResult();
                var modelId = this.GetOrCreateModelAsync(makeId, car.Model.Trim()).GetAwaiter().GetResult();
                var colorId = this.GetOrCreateColorAsync(car.ColorName.Trim()).GetAwaiter().GetResult();

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

                await this.carsRepository.AddAsync(newCar);
                await this.carsRepository.SaveChangesAsync();

                var extras = this.GetOrCreateExtrasAsync(car.ExtraNames, newCar.Id).GetAwaiter().GetResult();
                newCar.Extras = extras;

                await this.carsRepository.AddAsync(newCar);
            }

            // await this.AddCarToDbAsync(bag);
            await this.carsRepository.SaveChangesAsync();
        }

        private async Task AddCarToDbAsync(ConcurrentBag<CarDto> cars)
        {
            foreach (var car in cars)
            {
                if (car == null)
                {
                    continue;
                }

                var makeId = await this.GetOrCreateMakeAsync(car.Make.Trim());
                var modelId = await this.GetOrCreateModelAsync(makeId, car.Model.Trim());
                var colorId = await this.GetOrCreateColorAsync(car.ColorName.Trim());

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

                await this.carsRepository.AddAsync(newCar);
                await this.carsRepository.SaveChangesAsync();

                var extras = await this.GetOrCreateExtrasAsync(car.ExtraNames, newCar.Id);
                newCar.Extras = extras;

            }

            await this.carsRepository.SaveChangesAsync();
        }

        private async Task<ConcurrentBag<CarDto>> GetCars(int currentPage)
        {
            var carDtoBag = new ConcurrentBag<CarDto>();

            var page = await this.context
                .OpenAsync($"https://www.cars.bg/carslist.php?page={currentPage}");

            var carUrlQuery = page.QuerySelectorAll($"#listContainer > div > div > div > div > div > div > div > div:nth-child(2)");

            foreach (var entry in carUrlQuery)
            {
                var carUrl = entry.OuterHtml.Split("\"")[1];

                var car = this.GetData(carUrl);
                carDtoBag.Add(car);
            }

            return carDtoBag;
        }

        private CarDto GetData(string url)
        {
            var document = this.context.OpenAsync(url).GetAwaiter().GetResult();

            var carDataQuery = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div:nth-child(2) > h2");
            var carDataString = carDataQuery.TextContent.Split(" ", 3);

            var carDesc = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div.text-copy");
            var carDataArr = carDesc.TextContent.Trim().Split(',');

            // power, cubic,evro, nov vnos
            var carProductionMonth = EmptyString;
            var carProductionYear = EmptyString;
            var carCategory = EmptyString;
            var carEngineType = EmptyString;
            var carMileage = EmptyString;
            var carGearbox = EmptyString;
            var carColor = EmptyString;
            var carPower = EmptyString;
            var carEuroStandard = EmptyString;

            if (carDesc.TextContent.Contains("EURO") && carDesc.TextContent.Contains("см3") && carDesc.TextContent.Contains("к.с") && carDesc.TextContent.Contains("нов внос"))
            {
                carProductionMonth = carDataArr[0].Split()[0];
                carProductionYear = carDataArr[0].Split()[1];
                carCategory = carDataArr[1];
                carEngineType = carDataArr[5];
                carMileage = carDataArr[6];
                carGearbox = carDataArr[7];
                carPower = carDataArr[8];
                carEuroStandard = carDataArr[9];
                carColor = carDataArr[12];
            }
            else
            {
                return null;
            }

            //--------------------------------------------------------
            if (carEngineType.Contains("Дизел"))
            {
                carEngineType = "Diesel";
            }
            else if (carEngineType.Contains("Бензин"))
            {
                carEngineType = "Gasoline";
            }
            else if (carEngineType.Contains("Електричество"))
            {
                carEngineType = "Electric";
            }
            else
            {
                carEngineType = "Hybrid";
            }

            if (carGearbox.Contains("Ръчни"))
            {
                carGearbox = "Manual";
            }
            else if (carGearbox.Contains("Автоматични скорости"))
            {
                carGearbox = "Automatic";
            }

            carEuroStandard = carEuroStandard.Trim().Replace(" ", "_");

            carPower = carPower.Replace("к.с.", EmptyString).Trim();
            carMileage = carMileage.Replace(" ", EmptyString);
            carMileage = carMileage.Replace("км", EmptyString);

            var priceQuery = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div.offer-price > strong");
            var carPrice = priceQuery.TextContent.Replace(",", EmptyString).Replace("лв.", EmptyString);

            var locationQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(3) > table > tbody > tr > td");
            if (locationQuery == null)
            {
                locationQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(3)");
            }
            else
            {
                if (locationQuery == null)
                {
                    locationQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(4) > table > tbody > tr > td");
                }
            }

            var location = locationQuery.TextContent.Trim().Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            var carLocation = EmptyString;
            if (locationQuery.TextContent.Contains("Частно"))
            {
                carLocation = locationQuery.TextContent.Replace("Частно лице", EmptyString).Trim();
            }
            else
            {
                carLocation = location[location.Length - 1];
            }

            var carDescription = string.Empty;
            var carExtras = string.Empty;

            var carDescQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(1)");
            carDescription = carDescQuery.TextContent.Trim();

            if (carDescription.Contains("Особености и Екстри"))
            {
                carExtras = carDescription.Trim().Replace("Комфорт", EmptyString).Replace("Сигурност", EmptyString).Replace("Друго", EmptyString).Replace(":", EmptyString);
                carDescription = EmptyString;
            }
            else
            {
                var extrasQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(2) > div.description.text-copy");
                if (extrasQuery == null)
                {
                    extrasQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(3) > div.description.text-copy");
                }

                carExtras = extrasQuery.TextContent.Trim().Replace("Комфорт", EmptyString).Replace("Сигурност", EmptyString).Replace("Друго", EmptyString).Replace(":", EmptyString);
            }

            var carExtrasList = carExtras.Trim().Split(new string[] { "\n", "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();

            var imageQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-3-desktop > img:nth-child(2)");
            var carImgArr = imageQuery.OuterHtml.Split("\"");
            var carImageUrl = carImgArr[1];

            var carCurrency = EmptyString;

            if (carPrice.Contains("По"))
            {
                carPrice = "По договаряне";
                carCurrency = null;
            }
            else
            {
                carCurrency = "лв.";
            }

            var carMake = carDataString[0];
            var carModel = carDataString[1];
            var carModification = carDataString[2];

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

                if (extra != null || string.IsNullOrWhiteSpace(extraName))
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

        private async Task<List<CarsExtras>> GetOrCreateExtrasAsync(ICollection<string> extras, int carId)
        {
            var extrasToReturn = new List<CarsExtras>();

            foreach (var extraName in extras)
            {
                var extra = this.extrasRepository
                    .AllAsNoTracking().FirstOrDefault(x => x.Name == extraName);

                if (extra == null && !string.IsNullOrWhiteSpace(extraName))
                {
                    extra = new Extra
                    {
                        Name = extraName,
                    };

                    await this.extrasRepository.AddAsync(extra);
                    await this.extrasRepository.SaveChangesAsync();
                }

                var carExtra = new CarsExtras
                {
                    ExtraId = extra.Id,
                    CarId = carId,
                };

                extrasToReturn.Add(carExtra);

                await this.carsExtrasRepository.AddAsync(carExtra);
                await this.carsExtrasRepository.SaveChangesAsync();
            }

            return extrasToReturn;
        }
    }
}
