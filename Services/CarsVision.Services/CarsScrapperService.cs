namespace CarsVision.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
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

        public ConcurrentBag<RawPropery> PopulateDb(int pagesCount)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bag = new ConcurrentBag<RawPropery>();

            Parallel.For(1, pagesCount, (i) =>
            {
                var cars = this.GetCars(i);

                Parallel.ForEach(cars, (entry) =>
                {
                    if (entry != null)
                    {
                        bag.Add(entry);
                    }
                });
            });

            Console.WriteLine(stopwatch.Elapsed.TotalSeconds);
            Console.WriteLine(bag.Count());

            return bag;

            //foreach (var car in bag.Where(x => x != null))
            //{
            //    var makeId = await this.GetOrCreateMakeAsync(car.Make.Trim());
            //    var modelId = await this.GetOrCreateModelAsync(makeId, car.Model.Trim());

            //    var newCar = new Car
            //    {
            //        MakeId = makeId,
            //        ModelId = modelId,
            //        ColorId = 7,
            //        Modification = car.Modification,
            //        ImageUrl = car.ImageUrl,
            //        Mileage = car.Mileage,
            //        Power = car.Power,
            //        Price = car.Price,
            //        Location = car.Location,
            //        Year = car.Year,
            //        Description = car.Description,
            //        Currency = (Currency)Enum.Parse(typeof(Currency), car.Currency == "лв." ? "BGN" : car.Currency, true),
            //    };

            //    await this.dbContext.Cars.AddAsync(newCar);
            //    await this.dbContext.SaveChangesAsync();

            //    var extras = await this.GetOrCreateExtrasAsync(car.ExtraNames, newCar.Id);

            //    newCar.Extras = extras;

            //    await this.dbContext.SaveChangesAsync();
            //    Console.WriteLine($"added {car.Make} - {car.Model}");
            //}
            //Console.WriteLine(stopwatch.Elapsed.TotalMinutes);
        }

        private ConcurrentBag<RawPropery> GetCars(int currentPage)
        {
            var carDtoBag = new ConcurrentBag<RawPropery>();

            var page = this.context
                .OpenAsync($"https://www.cars.bg/carslist.php?page={currentPage}").GetAwaiter().GetResult();

            var carUrlQuery = page.QuerySelectorAll($"#listContainer > div > div > div > div > div > div > div > a");

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

        private RawPropery GetData(string url)
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

                //var locationQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(3) > table > tbody > tr > td");
                //if (locationQuery == null)
                //{
                //    locationQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(3)");
                //}
                //else
                //{
                //    if (locationQuery == null)
                //    {
                //        locationQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(4) > table > tbody > tr > td");
                //    }
                //}
                //
                //var location = locationQuery.TextContent.Trim().Split(new string[] { " ", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                //var carLocation = EmptyString;
                //if (locationQuery.TextContent.Contains("Частно"))
                //{
                //    carLocation = locationQuery.TextContent.Replace("Частно лице", EmptyString).Trim();
                //}
                //else
                //{
                //    carLocation = location[location.Length - 1];
                //}
                //
                //var carDescription = string.Empty;
                //var carExtras = string.Empty;
                //
                //var carDescQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(1)");
                //carDescription = carDescQuery.TextContent.Trim();
                //
                //if (carDescription.Contains("Особености и Екстри"))
                //{
                //    carExtras = carDescription.Trim().Replace("Комфорт", EmptyString).Replace("Сигурност", EmptyString).Replace("Друго", EmptyString).Replace(":", EmptyString);
                //    carDescription = EmptyString;
                //}
                //else
                //{
                //    var extrasQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(2) > div.description.text-copy");
                //    if (extrasQuery == null)
                //    {
                //        extrasQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-2-desktop > div > div:nth-child(3) > div.description.text-copy");
                //    }
                //
                //    carExtras = extrasQuery.TextContent.Trim().Replace("Комфорт", EmptyString).Replace("Сигурност", EmptyString).Replace("Друго", EmptyString).Replace(":", EmptyString);
                //}
                //
                //var carExtrasList = carExtras.Trim().Split(new string[] { "\n", "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
                //
                //var imageQuery = document.QuerySelector("#main-content > div > div:nth-child(2) > div > div.mdc-layout-grid__cell.mdc-layout-grid__cell--span-4-phone.mdc-layout-grid__cell--span-4-tablet.mdc-layout-grid__cell--span-3-desktop > img:nth-child(2)");
                //var carImageUrl = EmptyString;
                //if (imageQuery != null)
                //{
                //    var carImgArr = imageQuery.OuterHtml.Split("\"");
                //    carImageUrl = carImgArr[1];
                //}
                //else
                //{
                //    carImageUrl = EmptyString;
                //}
                //
                //var carCurrency = EmptyString;

                if (carPrice.Contains("По"))
                {
                    carPrice = "По договаряне";
                    //carCurrency = null;
                    return null;
                }
                //else
                //{
                //    carCurrency = "лв.";
                //}

                var makeMatch = carDataString[0];
                var modelMatch = carDataString[1];
                var carModification = carDataString[2];

                var myDesc = document.QuerySelector("#main-content > div > div:nth-child(1) > div > div.text-copy").TextContent;

                var makeRegex = new Regex(@"Mercedes-Benz", RegexOptions.Compiled);
                var modelRegex = new Regex(@"[A-Z]+? [0-9]+", RegexOptions.Compiled);

                if (makeMatch == "Mercedes-Benz")
                {
                    makeMatch = makeRegex.Match(carDataQuery.TextContent).Value;
                    modelMatch = modelRegex.Match(carDataQuery.TextContent).Value;
                }
                else if (makeMatch == "VW")
                {
                    makeMatch = "Volkswagen";
                }

                var yearRegex = new Regex(@"[0-9]+", RegexOptions.Compiled);
                var powerRegex = new Regex(@"[0-9]+к.с.", RegexOptions.Compiled);
                var mileageRegex = new Regex(@"[0-9]+ [ ]?[0-9]+км", RegexOptions.Compiled);
                var gearboxRegex = new Regex(@"[А-Яа-я]+ скорости", RegexOptions.Compiled);
                var euroRegex = new Regex(@"[EURO]+ [1-7]", RegexOptions.Compiled);

                var yearMatch = yearRegex.Match(myDesc).Value;
                var powerMatch = powerRegex.Match(myDesc).Value.Trim().Replace("к.с.", string.Empty);
                var mileageMatch = mileageRegex.Match(myDesc).Value.Trim().Replace(" ", string.Empty).Replace("км", string.Empty);
                var gearboxMatch = gearboxRegex.Match(myDesc).Value;
                var euroMatch = euroRegex.Match(myDesc).Value;

                var car = new RawPropery
                {
                    Url = url,
                    Make = makeMatch,
                    Model = modelMatch,
                    Year = int.Parse(yearMatch),
                    Mileage = int.Parse(mileageMatch),
                    Power = int.Parse(powerMatch),
                    Gearbox = gearboxMatch,
                    Eurostandard = euroMatch,
                    Price = int.Parse(carPrice),
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

    public class RawPropery
    {
        public string Url { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public int Year { get; set; }

        public int Power { get; set; }

        public int Mileage { get; set; }

        public string Eurostandard { get; set; }

        public string Gearbox { get; set; }

        public int Price { get; set; }
    }
}
