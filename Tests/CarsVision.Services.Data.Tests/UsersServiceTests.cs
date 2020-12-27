namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using Moq;
    using Xunit;

    public class UsersServiceTests
    {
        private readonly DateTime date;
        private readonly UsersService mockService;
        private readonly Mock<IRepository<Color>> mockColorRepository;
        private readonly Mock<IRepository<ApplicationUser>> mockUserRepository;
        private readonly Mock<IDeletableEntityRepository<Car>> mockCarRepository;
        private readonly Mock<IDeletableEntityRepository<Make>> mockMakeRepository;
        private readonly Mock<IDeletableEntityRepository<Watchlist>> mockWatchlistRepository;

        public UsersServiceTests()
        {
            this.date = DateTime.Now;
            this.mockColorRepository = new Mock<IRepository<Color>>();
            this.mockUserRepository = new Mock<IRepository<ApplicationUser>>();
            this.mockCarRepository = new Mock<IDeletableEntityRepository<Car>>();
            this.mockMakeRepository = new Mock<IDeletableEntityRepository<Make>>();
            this.mockWatchlistRepository = new Mock<IDeletableEntityRepository<Watchlist>>();
            this.mockService = new UsersService(
                this.mockCarRepository.Object,
                this.mockColorRepository.Object,
                this.mockMakeRepository.Object,
                this.mockUserRepository.Object,
                this.mockWatchlistRepository.Object);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public async Task AddToWatchlistShouldAdd()
        {
            this.mockUserRepository.Setup(x => x.All())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "id",
                        Watchlists = new List<Watchlist>(),
                    },
                }.AsQueryable());

            var result = await this.mockService.AddCarToWatchlist(23, "id");
            Assert.Equal("created", result);
        }

        [Fact]
        public async Task AddToWatchlistShouldRemove()
        {
            this.mockWatchlistRepository.Setup(x => x.All())
                .Returns(new List<Watchlist>()
                {
                    new Watchlist
                    {
                        CarId = 23,
                        UserId = "id",
                    },
                }.AsQueryable());

            this.mockUserRepository.Setup(x => x.All())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "id",
                        Watchlists = new List<Watchlist>() { new Watchlist { CarId = 23, UserId = "id" } },
                    },
                }.AsQueryable());

            var result = await this.mockService.AddCarToWatchlist(23, "id");
            Assert.Equal("deleted", result);
        }

        [Fact]
        public void GetCarByIdShouldReturnCorrectly()
        {
            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                        Id = 23,
                        Make = new Make { Name = "vw", },
                        Model = new Model { Id = 1, Name = "golf" },
                        Modification = "1.3",
                        Gearbox = Gearbox.Manual,
                        Currency = Currency.BGN,
                        EngineType = EngineType.Gasoline,
                        EuroStandard = EuroStandard.Euro_3,
                        Power = 55,
                        Year = 1988,
                        Location = "krg",
                        UserId = "userId",
                        CreatedOn = this.date,
                        Price = 500,
                        Description = "desc",
                    },
                }.AsQueryable());

            var car = this.mockService.GetCarById(23);
            Assert.Equal("vw", car.Make);
            Assert.Equal("golf", car.Model);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal("1988", car.Year);
            Assert.Equal(Gearbox.Manual, car.Gearbox);
            Assert.Equal(Currency.BGN, car.Currency);
            Assert.Equal(EngineType.Gasoline, car.EngineType);
            Assert.Equal(EuroStandard.Euro_3, car.EuroStandard);
            Assert.Equal("krg", car.Location);
            Assert.Equal(55, car.Power);
            Assert.Equal(500, car.Price);
            Assert.Equal("desc", car.Description);
        }

        [Fact]
        public void GetCountShouldReturnOne()
        {
            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                    new Car
                    {
                        Id = 23,
                        Make = new Make { Name = "vw", },
                        Model = new Model { Id = 1, Name = "golf" },
                        Modification = "1.3",
                        Gearbox = Gearbox.Manual,
                        Currency = Currency.BGN,
                        EngineType = EngineType.Gasoline,
                        EuroStandard = EuroStandard.Euro_3,
                        Power = 55,
                        Year = 1988,
                        Location = "krg",
                        UserId = "userId",
                        CreatedOn = this.date,
                        Price = 500,
                        Description = "desc",
                    },
                }.AsQueryable());

            var count = this.mockService.GetCount("userId");
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetCountShouldReturnZero()
        {
            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                }.AsQueryable());

            var count = this.mockService.GetCount("userId");
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetAllShouldReturnOneCar()
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

            this.mockWatchlistRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Watchlist>()
                {
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
                        Color = new Color() { Name = "white" },
                        Currency = Currency.BGN,
                        CreatedOn = this.date,
                        Price = 500,
                        Description = "desc",
                        Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                        User = new ApplicationUser() { Id = "userId", PhoneNumber = "088", },
                        UserId = "userId",
                    },
                }.AsQueryable());

            var cars = this.mockService.GetAll(1, 12, "userId");
            Assert.Single(cars);
        }

        [Fact]
        public void GetAllShouldReturnZeroCars()
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

            this.mockWatchlistRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Watchlist>()
                {
                }.AsQueryable());

            this.mockCarRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Car>()
                {
                }.AsQueryable());

            var cars = this.mockService.GetAll(1, 12, "userId");
            Assert.Empty(cars);
        }

        [Fact]
        public void GetAllShouldSetValuesOfTheCarsCorrectlly()
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

            this.mockWatchlistRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Watchlist>()
                {
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
                        Color = new Color() { Name = "white" },
                        Currency = Currency.BGN,
                        CreatedOn = this.date,
                        Price = 500,
                        Description = "desc",
                        Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                        User = new ApplicationUser() { Id = "userId", PhoneNumber = "088", },
                        UserId = "userId",
                    },
                }.AsQueryable());

            var car = this.mockService.GetAll(1, 12, "userId").FirstOrDefault();
            Assert.Equal(23, car.Id);
            Assert.Equal("vw", car.MakeName);
            Assert.Equal("golf", car.ModelName);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal("1988", car.Year);
            Assert.Equal(10, car.Mileage);
            Assert.Equal("white", car.ColorName);
            Assert.Equal(this.date, car.CreatedOn);
            Assert.Equal(500, car.Price);
            Assert.Equal("desc", car.Description);
            Assert.Equal("/images/cars/picId.ext", car.PictureUrl);
            Assert.Equal("088", car.UserPhoneNumber);
        }
    }
}
