namespace CarsVision.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    using AngleSharp;
    using CarsVision.Data;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Models;

    public class CarsScrapperService : ICarsScrapperService
    {
        private static readonly string EmptyString = string.Empty;

        private readonly IBrowsingContext context;
        private readonly IDeletableEntityRepository<Make> makesRepository;
        private readonly IDeletableEntityRepository<Model> modelsRepository;
        private readonly IDeletableEntityRepository<Extra> extrasRepository;
        private readonly IRepository<CarsExtras> carsExtrasRepository;
        private readonly ApplicationDbContext dbContext;

        public CarsScrapperService(
            IDeletableEntityRepository<Make> makesRepository,
            IDeletableEntityRepository<Model> modelsRepository,
            IDeletableEntityRepository<Extra> extrasRepository,
            IRepository<CarsExtras> carsExtrasRepository,
            ApplicationDbContext dbContext)
        {
            this.makesRepository = makesRepository;
            this.modelsRepository = modelsRepository;
            this.extrasRepository = extrasRepository;
            this.dbContext = dbContext;
            this.carsExtrasRepository = carsExtrasRepository;

            var config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(config);
        }

        public async Task PopulateDb(int pagesCount)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bag = new ConcurrentBag<CarDto>();

            Parallel.For(1, pagesCount, (i) =>
            {
                var cars = this.GetCars(i);

                Parallel.ForEach(cars, (entry) =>
                {
                    bag.Add(entry);
                });
            });

            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine(bag.Where(x => x != null).Count());

            foreach (var car in bag.Where(x => x != null))
            {
                var makeId = await this.GetOrCreateMakeAsync(car.Make.Trim());
                var modelId = await this.GetOrCreateModelAsync(makeId, car.Model.Trim());

                var newCar = new Car
                {
                    MakeId = makeId,
                    ModelId = modelId,
                    ColorId = 7,
                    Modification = car.Modification,
                    ImageUrl = car.ImageUrl,
                    Mileage = car.Mileage,
                    Power = car.Power,
                    Price = car.Price,
                    Location = car.Location,
                    Year = car.Year,
                    Description = car.Description,
                    Currency = (Currency)Enum.Parse(typeof(Currency), car.Currency == "лв." ? "BGN" : car.Currency, true),
                };

                await this.dbContext.Cars.AddAsync(newCar);
                await this.dbContext.SaveChangesAsync();

                var extras = await this.GetOrCreateExtrasAsync(car.ExtraNames, newCar.Id);

                newCar.Extras = extras;

                await this.dbContext.SaveChangesAsync();
                Console.WriteLine($"added {car.Make} - {car.Model}");
            }

            Console.WriteLine(stopwatch.Elapsed.TotalMinutes);
        }

        private ConcurrentBag<CarDto> GetCars(int currentPage)
        {
            var carDtoBag = new ConcurrentBag<CarDto>();

            var page = this.context
                .OpenAsync($"https://www.cars.bg/carslist.php?page={currentPage}").GetAwaiter().GetResult();

            var carUrlQuery = page.QuerySelectorAll($"#listContainer > div > div > div > div > div > div > div > div:nth-child(2)");

            foreach (var entry in carUrlQuery)
            {
                try
                {
                    var carUrl = entry.OuterHtml.Split("\"")[1];

                    var car = this.GetData(carUrl);
                    carDtoBag.Add(car);
                }
                catch (Exception)
                {
                    Console.WriteLine("error");
                }
            }

            return carDtoBag;
        }

        private CarDto GetData(string url)
        {
            try
            {
                var document = this.context.OpenAsync(url).GetAwaiter().GetResult();

                var carDataQuery = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div:nth-child(2) > h2");
                var carDataString = carDataQuery.TextContent.Split(" ", 3);

                var carDesc = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div.text-copy");
                var carDataArr = carDesc.TextContent.Trim().Split(',');

                var priceQuery = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div.offer-price");
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
                var carImageUrl = EmptyString;
                if (imageQuery != null)
                {
                    var carImgArr = imageQuery.OuterHtml.Split("\"");
                    carImageUrl = carImgArr[1];
                }
                else
                {
                    carImageUrl = EmptyString;
                }

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
                    Currency = carCurrency,
                    Location = carLocation,
                    Description = carModification + " ---Description--- " + carDescription,
                    ExtraNames = carExtrasList,
                };

                Console.WriteLine($"returned {car.Make} -- {car.Model}");
                return car;
            }
            catch (Exception)
            {
                Console.WriteLine("error 222");
                return null;
            }
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

        private async Task<List<CarsExtras>> GetOrCreateExtrasAsync(ICollection<string> extras, int carId)
        {
            var extrasToReturn = new List<CarsExtras>();

            foreach (var extraName in extras)
            {
                var extra = this.extrasRepository
                    .AllAsNoTracking().FirstOrDefault(x => x.Name == extraName);

                if (extra == null)
                {
                    if (string.IsNullOrWhiteSpace(extraName) || extraName == "Особености и Екстри")
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
