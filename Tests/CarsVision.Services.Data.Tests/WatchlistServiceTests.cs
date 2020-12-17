namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using CarsVision.Services.Mapping;
    using Microsoft.EntityFrameworkCore;
    using Moq;
    using Xunit;

    public class WatchlistServiceTests
    {
        private readonly DateTime date;
        private readonly WatchlistsService service;
        private readonly WatchlistsService mockService;
        private readonly ApplicationDbContext dbContext;
        private readonly IRepository<Watchlist> watchlistsRepository;
        private readonly Mock<IDeletableEntityRepository<Watchlist>> mockWatchlistRepository;

        public WatchlistServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);

            this.watchlistsRepository = new EfRepository<Watchlist>(this.dbContext);
            this.service = new WatchlistsService(this.watchlistsRepository);

            this.mockWatchlistRepository = new Mock<IDeletableEntityRepository<Watchlist>>();
            this.mockService = new WatchlistsService(this.mockWatchlistRepository.Object);
            this.date = DateTime.Now;
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public async Task AddShouldAddSuccessfully()
        {
            await this.service.Add(23, "userId");

            var count = await this.watchlistsRepository.All().CountAsync();
            var car = await this.watchlistsRepository.All().FirstAsync(x => x.CarId == 23);

            Assert.Equal(1, count);
            Assert.NotNull(car);
        }

        [Fact]
        public async Task AddShouldSetWatchlistValuesCorrectly()
        {
            await this.service.Add(23, "userId");

            var car = await this.watchlistsRepository.All().FirstAsync();

            Assert.Equal("userId", car.UserId);
            Assert.Equal(23, car.CarId);
        }

        [Fact]
        public async Task RemoveShouldRemoveWatchlistSuccessfully()
        {
            await this.service.Add(23, "userId");
            await this.service.Remove(23, "userId");

            var count = await this.watchlistsRepository.All().CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task GetCountShouldReturnRight()
        {
            await this.service.Add(23, "userId");
            var count = this.service.GetCount("userId");
            var countTwo = this.service.GetCount("userIdTwo");

            Assert.Equal(1, count);
            Assert.Equal(0, countTwo);
        }

        [Fact]
        public async Task IsInWatchlistShouldReturnTrue()
        {
            await this.service.Add(23, "userId");
            var result = this.service.IsInWatchlist(23, "userId");

            Assert.True(result);
        }

        [Fact]
        public void IsInWatchlistShouldReturnFalse()
        {
            var result = this.service.IsInWatchlist(23, "userId");

            Assert.False(result);
        }

        [Fact]
        public void GetAllShouldReturnSuccessfully()
        {
            this.mockWatchlistRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Watchlist>()
                {
                    new Watchlist
                    {
                        CarId = 23,
                        Car = new Car
                        {
                            Id = 23,
                            Make = new Make { Name = "vw", },
                            Model = new Model { Id = 1, Name = "golf" },
                            Modification = "1.3",
                            Color = new Color { Name = "white" },
                            CreatedOn = this.date,
                            Location = "krg",
                            Description = "desc",
                            Year = "1988",
                            Currency = Currency.BGN,
                            EngineType = EngineType.Gasoline,
                            EuroStandard = EuroStandard.Euro_3,
                            Gearbox = Gearbox.Manual,
                            Mileage = 10,
                            Power = 55,
                            Price = 500,
                            Views = 69,
                            UserId = "userId",
                            Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                        },
                        UserId = "userId",
                        User = new ApplicationUser
                        {
                            Id = "userId",
                            PhoneNumber = "088",
                        },
                    },
                }.AsQueryable());

            var cars = this.mockService.GetAll(1, 12, "userId");
            Assert.Single(cars);
        }

        [Fact]
        public void GetAllShouldSetValuesCorrectly()
        {
            this.mockWatchlistRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Watchlist>()
                {
                    new Watchlist
                    {
                        CarId = 23,
                        Car = new Car
                        {
                            Id = 23,
                            Make = new Make { Name = "vw", },
                            Model = new Model { Id = 1, Name = "golf" },
                            Modification = "1.3",
                            Color = new Color { Name = "white" },
                            CreatedOn = this.date,
                            Location = "krg",
                            Description = "desc",
                            Year = "1988",
                            Currency = Currency.BGN,
                            EngineType = EngineType.Gasoline,
                            EuroStandard = EuroStandard.Euro_3,
                            Gearbox = Gearbox.Manual,
                            Mileage = 10,
                            Power = 55,
                            Price = 500,
                            Views = 69,
                            UserId = "userId",
                            Pictures = new List<Picture> { new Picture { Id = "picId", Extension = "ext" } },
                        },
                        UserId = "userId",
                        User = new ApplicationUser
                        {
                            Id = "userId",
                            PhoneNumber = "088",
                        },
                    },
                }.AsQueryable());

            var car = this.mockService.GetAll(1, 12, "userId").FirstOrDefault();
            Assert.True(car.IsInWatchlist);
            Assert.Equal("vw", car.MakeName);
            Assert.Equal("golf", car.ModelName);
            Assert.Equal("1.3", car.Modification);
            Assert.Equal("1988", car.Year);
            Assert.Equal("krg", car.Location);
            Assert.Equal(10, car.Mileage);
            Assert.Equal("white", car.ColorName);
            Assert.Equal("088", car.UserPhoneNumber);
            Assert.Equal(500, car.Price);
            Assert.Equal("BGN", car.Currency);
            Assert.Equal("desc", car.Description);
            Assert.Equal("/images/cars/picId.ext", car.PictureUrl);
        }
    }
}
