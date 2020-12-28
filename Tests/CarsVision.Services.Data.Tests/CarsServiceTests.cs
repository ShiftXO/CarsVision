namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class CarsServiceTests
    {
        private readonly DateTime date;
        private readonly CarsService mockService;
        private readonly Mock<CommonService> mockCommonService;
        private readonly Mock<IRepository<Color>> mockColorRepository;
        private readonly Mock<IRepository<Watchlist>> mockWatchlistRepository;
        private readonly Mock<IDeletableEntityRepository<Car>> mockCarRepository;
        private readonly Mock<IDeletableEntityRepository<Make>> mockMakeRepository;
        private readonly Mock<IDeletableEntityRepository<ApplicationUser>> mockUserRepository;
        private readonly Mock<IDeletableEntityRepository<Dealership>> mockDealershipRepository;
        private readonly Mock<IRepository<Extra>> mockExtraRepository;

        public CarsServiceTests()
        {
            this.date = DateTime.Now;
            this.mockCommonService = new Mock<CommonService>();
            this.mockWatchlistRepository = new Mock<IRepository<Watchlist>>();
            this.mockCarRepository = new Mock<IDeletableEntityRepository<Car>>();
            this.mockUserRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            this.mockDealershipRepository = new Mock<IDeletableEntityRepository<Dealership>>();
            this.mockMakeRepository = new Mock<IDeletableEntityRepository<Make>>();
            this.mockColorRepository = new Mock<IRepository<Color>>();
            this.mockExtraRepository = new Mock<IRepository<Extra>>();
            this.mockService = new CarsService(
                this.mockCarRepository.Object,
                this.mockMakeRepository.Object,
                this.mockColorRepository.Object,
                this.mockDealershipRepository.Object,
                this.mockUserRepository.Object,
                this.mockWatchlistRepository.Object,
                this.mockExtraRepository.Object,
                this.mockCommonService.Object);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public async Task AddCarShouldWork()
        {
            var cars = new List<Car>
            {
            };

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>
                {
                }.AsQueryable());

            this.mockMakeRepository.Setup(x => x.All())
                .Returns(new List<Make>
                {
                    new Make
                    {
                        Id = 1,
                        Name = "vw",
                        Models = new List<Model>
                        {
                            new Model { Id = 1, Name = "golf" },
                        },
                    },
                }.AsQueryable());

            this.mockColorRepository.Setup(x => x.All())
                .Returns(new List<Color>
                {
                    new Color { Id = 1, Name = "white" },
                }.AsQueryable());

            // Arrange
            var fileMock = new Mock<IFormFile>();

            // Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.png";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            this.mockCarRepository.Setup(r => r.AddAsync(It.IsAny<Car>())).Callback((Car workout) => cars.Add(workout));
            var inputModel = new CreateCarInputModel
            {
                Make = "vw",
                Model = "golf",
                Modification = "1.3",
                EngineType = EngineType.Gasoline,
                Power = 55,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Category = Category.Hatchback,
                Price = 500,
                Currency = Currency.BGN,
                Month = 2,
                Year = 1988,
                Mileage = 150000,
                IsVIP = false,
                Color = "white",
                Location = "krg",
                Description = "desc",
                Pictures = new List<IFormFile> { file },
            };

            await this.mockService.AddCarAsync(inputModel, "userId", "wwwroot/images/cars/");
            Assert.Contains(cars, x => x.MakeId == 1);
            Assert.Contains(cars, x => x.ModelId == 1);
            Assert.Contains(cars, x => x.Year == 1988);
            Assert.Single(cars);
        }

        [Fact]
        public async Task AddCarShouldThrow()
        {
            var cars = new List<Car>
            {
            };

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>
                {
                }.AsQueryable());

            this.mockMakeRepository.Setup(x => x.All())
                .Returns(new List<Make>
                {
                    new Make
                    {
                        Id = 1,
                        Name = "vw",
                        Models = new List<Model>
                        {
                            new Model { Id = 1, Name = "golf" },
                        },
                    },
                }.AsQueryable());

            this.mockColorRepository.Setup(x => x.All())
                .Returns(new List<Color>
                {
                    new Color { Id = 1, Name = "white" },
                }.AsQueryable());

            // Arrange
            var fileMock = new Mock<IFormFile>();

            // Setup mock file using a memory stream
            var content = "Hello World from a Fake File";
            var fileName = "test.xd";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            this.mockCarRepository.Setup(r => r.AddAsync(It.IsAny<Car>())).Callback((Car workout) => cars.Add(workout));
            var inputModel = new CreateCarInputModel
            {
                Make = "vw",
                Model = "golf",
                Modification = "1.3",
                EngineType = EngineType.Gasoline,
                Power = 55,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Category = Category.Hatchback,
                Price = 500,
                Currency = Currency.BGN,
                Month = 2,
                Year = 1988,
                Mileage = 150000,
                IsVIP = false,
                Color = "white",
                Location = "krg",
                Description = "desc",
                Pictures = new List<IFormFile> { file },
            };

            var ex = await Assert.ThrowsAsync<Exception>(async () => await this.mockService.AddCarAsync(inputModel, "userId", "wwwroot/images/dealerships/"));

            Assert.Equal("Invalid picture extension xd", ex.Message);
        }

        [Fact]
        public void GetAllShouldReturnCorrectllySingleCar()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Year = 1988,
                         Location = "krg",
                         Mileage = 10,
                         Color = new Color { Name = "white" },
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         CreatedOn = this.date,
                         Price = 500,
                         Currency = Currency.BGN,
                         Description = "desc",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var cars = this.mockService.GetAll(1, "userId", string.Empty, 12);
            Assert.Single(cars);
        }

        [Fact]
        public void GetAllShouldReturnCorrectllySingleCarOnSecondPage()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Year = 1988,
                         Location = "krg",
                         Mileage = 10,
                         Color = new Color { Name = "white" },
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         CreatedOn = this.date,
                         Price = 500,
                         Currency = Currency.BGN,
                         Description = "desc",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                    new Car
                    {
                         Id = 24,
                         Make = new Make { Name = "bmw", },
                         Model = new Model { Id = 1, Name = "e60" },
                         Modification = "3.0",
                         Year = 2006,
                         Location = "krg",
                         Mileage = 10,
                         Color = new Color { Name = "white" },
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         CreatedOn = this.date,
                         Price = 10000,
                         Currency = Currency.BGN,
                         Description = "desc",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var cars = this.mockService.GetAll(2, "userId", null, 1);
            Assert.Single(cars);
        }

        [Fact]
        public void GetAllShouldSetValuesCorrectlly()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Year = 1988,
                         Location = "krg",
                         Mileage = 10,
                         Color = new Color { Name = "white" },
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         CreatedOn = this.date,
                         Price = 500,
                         Currency = Currency.BGN,
                         Description = "desc",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var car = this.mockService.GetAll(1, "userId", null, 12).FirstOrDefault();
            Assert.Equal(23, car.Id);
            Assert.Equal("vw", car.MakeName);
            Assert.Equal("golf", car.ModelName);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal(1988, car.Year);
            Assert.Equal("krg", car.Location);
            Assert.Equal(10, car.Mileage);
            Assert.Equal("white", car.ColorName);
            Assert.Equal("088", car.UserPhoneNumber);
            Assert.Equal(this.date, car.CreatedOn);
            Assert.Equal(500, car.Price);
            Assert.Equal("BGN", car.Currency);
            Assert.Equal("desc", car.Description);
            Assert.Equal("/images/cars/picId.ext", car.PictureUrl);
            Assert.Equal("vw golf 1.3...", car.CarTitle);
            Assert.False(car.IsInWatchlist);
        }

        [Fact]
        public void GetAllShouldSetValuesCorrectllyWhenCarInWatchlist()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockWatchlistRepository.Setup(x => x.All())
                .Returns(new List<Watchlist>()
                {
                    new Watchlist
                    {
                        CarId = 23,
                        UserId = "userId",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Year = 1988,
                         Location = "krg",
                         Mileage = 10,
                         Color = new Color { Name = "white" },
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         CreatedOn = this.date,
                         Price = 500,
                         Currency = Currency.BGN,
                         Description = "desc",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var car = this.mockService.GetAll(1, "userId", null, 12).FirstOrDefault();
            Assert.Equal(23, car.Id);
            Assert.Equal("vw", car.MakeName);
            Assert.Equal("golf", car.ModelName);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal(1988, car.Year);
            Assert.Equal("krg", car.Location);
            Assert.Equal(10, car.Mileage);
            Assert.Equal("white", car.ColorName);
            Assert.Equal("088", car.UserPhoneNumber);
            Assert.Equal(this.date, car.CreatedOn);
            Assert.Equal(500, car.Price);
            Assert.Equal("BGN", car.Currency);
            Assert.Equal("desc", car.Description);
            Assert.Equal("/images/cars/picId.ext", car.PictureUrl);
            Assert.Equal("vw golf 1.3...", car.CarTitle);
            Assert.True(car.IsInWatchlist);
        }

        [Fact]
        public void GetAllMakesAndColorsShouldReturnCorrectlly()
        {
            this.mockMakeRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Make>()
                {
                    new Make
                    {
                        Name = "vw",
                    },
                    new Make
                    {
                        Name = "audi",
                    },
                }.AsQueryable());

            this.mockColorRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Color>()
                {
                    new Color
                    {
                        Name = "white",
                    },
                }.AsQueryable());

            var result = this.mockService.GetAllMakesAndColors();
            Assert.Equal(2, result.Makes.Count());
            Assert.Single(result.Colors);
        }

        [Fact]
        public void GetAllShouldReturnSuccessfullyWhenCarIsMadeByUser()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 1988,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 55,
                         Price = 500,
                         Views = 69,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var car = this.mockService.GetById(23);
            Assert.Equal(23, car.Id);
            Assert.Equal("vw", car.MakeName);
            Assert.Equal("golf", car.ModelName);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal("white", car.ColorName);
            Assert.Equal(this.date, car.CreatedOn);
            Assert.Equal("krg", car.Location);
            Assert.Equal("desc", car.Description);
            Assert.Equal(1988, car.Year);
            Assert.Equal("BGN", car.Currency.ToString());
            Assert.Equal("Gasoline", car.EngineType.ToString());
            Assert.Equal("Euro_3", car.EuroStandard.ToString());
            Assert.Equal("Manual", car.Gearbox.ToString());
            Assert.Equal(10, car.Mileage);
            Assert.Equal(55, car.Power);
            Assert.Equal(500, car.Price);
            Assert.Equal(69, car.Views);
            Assert.Equal("picId.ext", car.PictureUrls.First());
            Assert.Equal("088", car.PhoneNumber);
            Assert.Equal("userId", car.UserId);
            Assert.False(car.IsDealership);
        }

        [Fact]
        public void GetAllShouldReturnSuccessfullyWhenCarIsMadeByDealership()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "dealershipId",
                        PhoneNumber = "089",
                        Name = "autoa",
                        UserId = "userId",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 1988,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 55,
                         Price = 500,
                         Views = 69,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var car = this.mockService.GetById(23);
            Assert.Equal(23, car.Id);
            Assert.Equal("vw", car.MakeName);
            Assert.Equal("golf", car.ModelName);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal("white", car.ColorName);
            Assert.Equal(this.date, car.CreatedOn);
            Assert.Equal("krg", car.Location);
            Assert.Equal("desc", car.Description);
            Assert.Equal(1988, car.Year);
            Assert.Equal("BGN", car.Currency.ToString());
            Assert.Equal("Gasoline", car.EngineType.ToString());
            Assert.Equal("Euro_3", car.EuroStandard.ToString());
            Assert.Equal("Manual", car.Gearbox.ToString());
            Assert.Equal(10, car.Mileage);
            Assert.Equal(55, car.Power);
            Assert.Equal(500, car.Price);
            Assert.Equal(69, car.Views);
            Assert.Equal("picId.ext", car.PictureUrls.First());
            Assert.Equal("089", car.PhoneNumber);
            Assert.Equal("autoa", car.DealershipName);
            Assert.Equal("userId", car.UserId);
            Assert.True(car.IsDealership);
        }

        [Fact]
        public void GetCountShouldReturnCorrectlly()
        {
            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car { },
                    new Car { },
                    new Car { },
                }.AsQueryable());

            var count = this.mockService.GetCount();
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task UpdateShouldWorkCorrectlly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var carsRepository = new EfDeletableEntityRepository<Car>(dbContext);
            var makeRepository = new EfDeletableEntityRepository<Make>(dbContext);
            var colorRepository = new EfRepository<Color>(dbContext);
            var dealershipRepository = new EfDeletableEntityRepository<Dealership>(dbContext);
            var userRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var watchlistRepository = new EfDeletableEntityRepository<Watchlist>(dbContext);
            var extrasRepository = new EfRepository<Extra>(dbContext);
            var mockCommonService = new Mock<ICommonService>();

            var service = new CarsService(
                carsRepository,
                makeRepository,
                colorRepository,
                dealershipRepository,
                userRepository,
                watchlistRepository,
                extrasRepository,
                mockCommonService.Object);

            var date = DateTime.Now;
            var car = new Car
            {
                Id = 23,
                MakeId = 1,
                ModelId = 1,
                Modification = "1.3",
                ColorId = 1,
                CreatedOn = date,
                Location = "krg",
                Description = "desc",
                Year = 1988,
                Currency = Currency.BGN,
                EngineType = EngineType.Gasoline,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Mileage = 10,
                Power = 55,
                Price = 500,
                Views = 69,
                UserId = "userId",
            };
            var color = new Color
            {
                Id = 1,
                Name = "white",
            };
            var color1 = new Color
            {
                Id = 2,
                Name = "black",
            };
            var make = new Make
            {
                Id = 1,
                Name = "vw",
                Models = new List<Model>
                {
                    new Model { Id = 11, Name = "golf" },
                    new Model { Id = 12, Name = "bora" },
                },
            };
            var make1 = new Make
            {
                Id = 2,
                Name = "audi",
                Models = new List<Model>
                {
                    new Model { Id = 13, Name = "a5" },
                    new Model { Id = 14, Name = "a8" },
                },
            };

            await dbContext.AddAsync(car);
            await dbContext.AddAsync(color);
            await dbContext.AddAsync(color1);
            await dbContext.AddAsync(make);
            await dbContext.AddAsync(make1);

            await dbContext.SaveChangesAsync();
            var viewModel = new CarEditViewModel
            {
                Id = 23,
                Make = "audi",
                Model = "a5",
                Modification = "3.0tdi",
                EngineType = EngineType.Diesel,
                Power = 240,
                Gearbox = Gearbox.Automatic,
                Price = 20000,
                Currency = Currency.BGN,
                Month = "Feb",
                Year = "2010",
                Mileage = 150,
                Color = "black",
                Location = "sofiq",
                Description = "xdxd",
            };

            await service.Update(viewModel);
            var dbCar = dbContext.Cars.Find(23);
            Assert.Equal("audi", dbCar.Make.Name);
            Assert.Equal("a5", dbCar.Model.Name);
            Assert.Equal("3.0tdi", dbCar.Modification);
            Assert.Equal(240, dbCar.Power);
            Assert.Equal("Automatic", dbCar.Gearbox.ToString());
            Assert.Equal(20000, dbCar.Price);
            Assert.Equal("BGN", dbCar.Currency.ToString());
            Assert.Equal(2010, dbCar.Year);
            Assert.Equal(150, dbCar.Mileage);
            Assert.Equal("black", dbCar.Color.Name);
            Assert.Equal("sofiq", dbCar.Location);
            Assert.Equal("xdxd", dbCar.Description);
        }

        [Fact]
        public async Task DeleteShouldWorkCorrectlly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var carsRepository = new EfDeletableEntityRepository<Car>(dbContext);
            var makeRepository = new EfDeletableEntityRepository<Make>(dbContext);
            var colorRepository = new EfRepository<Color>(dbContext);
            var dealershipRepository = new EfDeletableEntityRepository<Dealership>(dbContext);
            var userRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var watchlistRepository = new EfDeletableEntityRepository<Watchlist>(dbContext);
            var extrasRepository = new EfRepository<Extra>(dbContext);
            var mockCommonService = new Mock<ICommonService>();

            var service = new CarsService(
                carsRepository,
                makeRepository,
                colorRepository,
                dealershipRepository,
                userRepository,
                watchlistRepository,
                extrasRepository,
                mockCommonService.Object);

            var date = DateTime.Now;
            var car = new Car
            {
                Id = 23,
                MakeId = 1,
                ModelId = 1,
                Modification = "1.3",
                ColorId = 1,
                CreatedOn = date,
                Location = "krg",
                Description = "desc",
                Year = 1988,
                Currency = Currency.BGN,
                EngineType = EngineType.Gasoline,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Mileage = 10,
                Power = 55,
                Price = 500,
                Views = 69,
                UserId = "userId",
            };
            var car1 = new Car
            {
                Id = 24,
                MakeId = 1,
                ModelId = 1,
                Modification = "1.3",
                ColorId = 1,
                CreatedOn = date,
                Location = "krg",
                Description = "desc",
                Year = 1988,
                Currency = Currency.BGN,
                EngineType = EngineType.Gasoline,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Mileage = 10,
                Power = 55,
                Price = 500,
                Views = 69,
                UserId = "userId",
            };
            await dbContext.AddAsync(car);
            await dbContext.AddAsync(car1);
            await dbContext.SaveChangesAsync();

            await service.Delete(23, "userId");
            var dbCarsCount = dbContext.Cars.ToList().Count();
            Assert.Equal(1, dbCarsCount);
        }

        [Fact]
        public async Task IncreaseViewsShouldWorkCorrectly()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var carsRepository = new EfDeletableEntityRepository<Car>(dbContext);
            var makeRepository = new EfDeletableEntityRepository<Make>(dbContext);
            var colorRepository = new EfRepository<Color>(dbContext);
            var dealershipRepository = new EfDeletableEntityRepository<Dealership>(dbContext);
            var userRepository = new EfDeletableEntityRepository<ApplicationUser>(dbContext);
            var watchlistRepository = new EfDeletableEntityRepository<Watchlist>(dbContext);
            var extrasRepository = new EfRepository<Extra>(dbContext);
            var mockCommonService = new Mock<ICommonService>();

            var service = new CarsService(
                carsRepository,
                makeRepository,
                colorRepository,
                dealershipRepository,
                userRepository,
                watchlistRepository,
                extrasRepository,
                mockCommonService.Object);

            var date = DateTime.Now;
            var car = new Car
            {
                Id = 23,
                MakeId = 1,
                ModelId = 1,
                Modification = "1.3",
                ColorId = 1,
                CreatedOn = date,
                Location = "krg",
                Description = "desc",
                Year = 1988,
                Currency = Currency.BGN,
                EngineType = EngineType.Gasoline,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Mileage = 10,
                Power = 55,
                Price = 500,
                Views = 69,
                UserId = "userId",
            };
            var car1 = new Car
            {
                Id = 24,
                MakeId = 1,
                ModelId = 1,
                Modification = "1.3",
                ColorId = 1,
                CreatedOn = date,
                Location = "krg",
                Description = "desc",
                Year = 1988,
                Currency = Currency.BGN,
                EngineType = EngineType.Gasoline,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Mileage = 10,
                Power = 55,
                Price = 500,
                Views = 0,
                UserId = "userId",
            };
            await dbContext.AddAsync(car);
            await dbContext.AddAsync(car1);
            await dbContext.SaveChangesAsync();

            var res = await service.IncreaseViews(23);
            var res1 = await service.IncreaseViews(24);
            var dbCarsCount = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == 23);
            var dbCarsCount1 = await dbContext.Cars.FirstOrDefaultAsync(x => x.Id == 24);
            Assert.Equal(70, dbCarsCount.Views);
            Assert.Equal(1, dbCarsCount1.Views);
            Assert.Equal(70, res);
            Assert.Equal(1, res1);
        }

        [Fact]
        public void SearchCarsShouldReturnCorrectlyWithDefaultFilter()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 1988,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 55,
                         Price = 500,
                         Views = 69,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                    new Car
                    {
                         Id = 24,
                         Make = new Make { Name = "bmw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 2000,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 555,
                         Price = 5500,
                         Views = 695,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var search = new CarsSearchInputModel()
            {
                Make = "vw",
                Model = "golf",
                MinPrice = 300,
                MaxPrice = 500,
                Order = "Make/Model/Price",
                EngineType = EngineType.Gasoline,
                Gearbox = Gearbox.Manual,
            };

            var result = this.mockService.SearchCars(search, "userId", 1, 12);
            Assert.Equal(1, result.Count);
            Assert.Equal(23, result.Cars.FirstOrDefault().Id);
            Assert.Equal("vw", result.Cars.FirstOrDefault().MakeName);
            Assert.Equal("golf", result.Cars.FirstOrDefault().ModelName);
            Assert.Equal("1.3", result.Cars.FirstOrDefault().Modification);
            Assert.Equal("white", result.Cars.FirstOrDefault().ColorName);
            Assert.Equal(this.date, result.Cars.FirstOrDefault().CreatedOn);
            Assert.Equal("krg", result.Cars.FirstOrDefault().Location);
            Assert.Equal("desc", result.Cars.FirstOrDefault().Description);
            Assert.Equal(1988, result.Cars.FirstOrDefault().Year);
            Assert.Equal("BGN", result.Cars.FirstOrDefault().Currency.ToString());
            Assert.Equal("Gasoline", result.Cars.FirstOrDefault().EngineType.ToString());
            Assert.Equal("Manual", result.Cars.FirstOrDefault().Gearbox.ToString());
            Assert.Equal(10, result.Cars.FirstOrDefault().Mileage);
            Assert.Equal(500, result.Cars.FirstOrDefault().Price);
            Assert.Equal("/images/cars/picId.ext", result.Cars.FirstOrDefault().PictureUrl);
            Assert.Equal("088", result.Cars.FirstOrDefault().UserPhoneNumber);
        }

        [Fact]
        public void SearchCarsShouldReturnCorrectlyWithPriceAscFilter()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 1988,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 55,
                         Price = 500,
                         Views = 69,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                    new Car
                    {
                         Id = 24,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "bora" },
                         Modification = "1.9",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 2000,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 1010,
                         Power = 150,
                         Price = 5500,
                         Views = 695,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var search = new CarsSearchInputModel()
            {
                Make = null,
                Model = null,
                MinPrice = 300,
                MaxPrice = 500,
                Order = "Price Asc.",
                EngineType = EngineType.Gasoline,
                Gearbox = Gearbox.Manual,
            };

            var result = this.mockService.SearchCars(search, "userId", 1, 12);
            Assert.Equal(1, result.Count);
            Assert.Equal(23, result.Cars.FirstOrDefault().Id);
        }

        [Fact]
        public void SearchCarsShouldReturnCorrectlyWithPriceDescFilter()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 1988,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 55,
                         Price = 500,
                         Views = 69,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                    new Car
                    {
                         Id = 24,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "bora" },
                         Modification = "1.9",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 2000,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 1010,
                         Power = 150,
                         Price = 5500,
                         Views = 695,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var search = new CarsSearchInputModel()
            {
                Make = null,
                Model = null,
                MinPrice = 0,
                MaxPrice = 0,
                Order = "Price Desc.",
                EngineType = EngineType.Gasoline,
                Gearbox = Gearbox.Manual,
            };

            var result = this.mockService.SearchCars(search, "userId", 1, 12);
            Assert.Equal(2, result.Count);
            Assert.Equal(24, result.Cars.FirstOrDefault().Id);
        }

        [Fact]
        public void SearchCarsShouldReturnCorrectlyWithMileageFilter()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                         Id = 23,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "golf" },
                         Modification = "1.3",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 1988,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 10,
                         Power = 55,
                         Price = 500,
                         Views = 69,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                    new Car
                    {
                         Id = 24,
                         Make = new Make { Name = "vw", },
                         Model = new Model { Id = 1, Name = "bora" },
                         Modification = "1.9",
                         Color = new Color { Name = "white" },
                         CreatedOn = this.date,
                         Location = "krg",
                         Description = "desc",
                         Year = 2000,
                         Currency = Currency.BGN,
                         EngineType = EngineType.Gasoline,
                         EuroStandard = EuroStandard.Euro_3,
                         Gearbox = Gearbox.Manual,
                         Mileage = 1010,
                         Power = 150,
                         Price = 5500,
                         Views = 695,
                         User = new ApplicationUser { Id = "userId", PhoneNumber = "088", },
                         UserId = "userId",
                         Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                    },
                }.AsQueryable());

            var search = new CarsSearchInputModel()
            {
                Make = null,
                Model = null,
                MinPrice = 0,
                MaxPrice = 0,
                Order = "Mileage",
                EngineType = EngineType.Gasoline,
                Gearbox = Gearbox.Manual,
            };

            var result = this.mockService.SearchCars(search, "userId", 1, 12);
            Assert.Equal(2, result.Count);
            Assert.Equal(23, result.Cars.FirstOrDefault().Id);
        }
    }
}
