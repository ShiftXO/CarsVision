namespace CarsVision.Services
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AngleSharp;
    using CarsVision.Services.Models;

    public class CarsScraperService : ICarsScrapperService
    {
        private static readonly string EmptyString = string.Empty;

        private readonly IConfiguration config;
        private readonly IBrowsingContext context;

        public CarsScraperService()
        {
            this.config = Configuration.Default.WithDefaultLoader();
            this.context = BrowsingContext.New(this.config);
        }

        public void PopulateDb(int pagesCount)
        {
            var bag = new ConcurrentBag<CarDto>();
            Parallel.For(1, pagesCount, (i) =>
            {
                try
                {
                    var cars = this.GetCar(i);
                    Parallel.ForEach(cars, (car) =>
                     {
                         bag.Add(car);
                     });
                }
                catch
                {
                }
            });

            // TODO: Populate Db
        }

        private ConcurrentBag<CarDto> GetCar(int id)
        {
            var bag = new ConcurrentBag<CarDto>();

            var page = this.context
                .OpenAsync($"https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=hs49ek&f1={id}")
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
                carPrice = priceData[0];
                carCurrency = priceData[1];
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
    }
}
