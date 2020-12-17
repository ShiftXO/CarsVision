namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Dealerships;
    using Microsoft.AspNetCore.Http;
    using Moq;
    using Xunit;

    public class DealershipsServiceTests
    {
        private readonly DateTime date;
        private readonly DealershipsService mockService;
        private readonly Mock<CommonService> mockCommonService;
        private readonly Mock<IRepository<Watchlist>> mockWatchlistRepository;
        private readonly Mock<IDeletableEntityRepository<Car>> mockCarRepository;
        private readonly Mock<IDeletableEntityRepository<ApplicationUser>> mockUserRepository;
        private readonly Mock<IDeletableEntityRepository<Dealership>> mockDealershipRepository;

        public DealershipsServiceTests()
        {
            this.date = DateTime.Now;
            this.mockCommonService = new Mock<CommonService>();
            this.mockWatchlistRepository = new Mock<IRepository<Watchlist>>();
            this.mockCarRepository = new Mock<IDeletableEntityRepository<Car>>();
            this.mockUserRepository = new Mock<IDeletableEntityRepository<ApplicationUser>>();
            this.mockDealershipRepository = new Mock<IDeletableEntityRepository<Dealership>>();

            this.mockService = new DealershipsService(
                this.mockDealershipRepository.Object,
                this.mockCarRepository.Object,
                this.mockUserRepository.Object,
                this.mockWatchlistRepository.Object,
                this.mockCommonService.Object);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public async Task CreateDealershipShouldWorkWithCorrectData()
        {
            var dealerships = new List<Dealership>();
            this.mockDealershipRepository.Setup(r => r.AllAsNoTracking()).Returns(() => dealerships.AsQueryable());
            this.mockDealershipRepository.Setup(r => r.AddAsync(It.IsAny<Dealership>())).Callback((Dealership dealership) => dealerships.Add(dealership));

            var fileMock = new Mock<IFormFile>();
            var content = "Hello World from a Fake File";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);

            var file = fileMock.Object;

            var model = new CreateDealershipInputModel
            {
                DealershipName = "autoa",
                Location = "sofiq",
                PhoneNumber = "088",
                Description = "desc",
                LogoPicture = file,
            };

            var appUser = new ApplicationUser
            {
                Email = "test@abv.bg",
            };

            var result = await this.mockService.CreateDealershipAsync(model, appUser, "wwwroot/images/dealerships/");
            Assert.Single(dealerships);
            Assert.True(result);
        }

        [Fact]
        public async Task CreateDealershipShouldThrowForPictureExtension()
        {
            var dealerships = new List<Dealership>();
            this.mockDealershipRepository.Setup(r => r.AllAsNoTracking()).Returns(() => dealerships.AsQueryable());
            this.mockDealershipRepository.Setup(r => r.AddAsync(It.IsAny<Dealership>())).Callback((Dealership dealership) => dealerships.Add(dealership));

            var fileMock = new Mock<IFormFile>();
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

            var model = new CreateDealershipInputModel
            {
                DealershipName = "autoa",
                Location = "sofiq",
                PhoneNumber = "088",
                Description = "desc",
                LogoPicture = file,
            };

            var appUser = new ApplicationUser
            {
                Email = "test@abv.bg",
            };

            var ex = await Assert.ThrowsAsync<Exception>(async () => await this.mockService.CreateDealershipAsync(model, appUser, "wwwroot/images/dealerships/"));

            Assert.Equal("Invalid picture extension xd", ex.Message);
        }

        [Fact]
        public void GetCountShouldReturnOne()
        {
            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Name = "automania",
                    },
                }.AsQueryable());

            var count = this.mockService.GetCount();
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetCountShouldReturnZero()
        {
            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                }.AsQueryable());

            var count = this.mockService.GetCount();
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetDealershipCarsCountShouldReturnZero()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        Cars = new List<Car>(),
                    },
                }.AsQueryable());
            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        UserId = "userId",
                    },
                }.AsQueryable());

            var count = this.mockService.GetDealershipsCarsCount("userId");
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetDealershipCarsCountShouldReturnOne()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        Cars = new List<Car>() { new Car { } },
                    },
                }.AsQueryable());
            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        UserId = "userId",
                    },
                }.AsQueryable());

            var count = this.mockService.GetDealershipsCarsCount("userId");
            Assert.Equal(1, count);
        }

        [Fact]
        public void GetDealershipInfoShouldReturnCorrectlly()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        Cars = new List<Car>() { new Car { } },
                    },
                }.AsQueryable());

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                }.AsQueryable());

            var dealer = this.mockService.GetDealershipInfo("userId");
            Assert.Equal("id", dealer.Id);
            Assert.Equal("userId", dealer.UserId);
            Assert.Equal("autoa", dealer.Name);
            Assert.Equal("Sofiq", dealer.Location);
            Assert.Equal(this.date, dealer.CreatedOn);
            Assert.Equal("/images/dealerships/picId.ext", dealer.LogoPicture);
            Assert.Equal("000", dealer.PhoneNumber);
            Assert.Equal(5, dealer.AverageVote);
        }

        [Fact]
        public void GetAllDealershipCarsShouldReturnCorrectllyZeroCars()
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
                }.AsQueryable());

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                }.AsQueryable());

            var dealerCars = this.mockService.GetAllDealershipCars(1, "userId", "id", null, 12);
            Assert.Empty(dealerCars);
        }

        [Fact]
        public void GetAllDealershipCarsShouldReturnCorrectllySingleCar()
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
                         Year = "1988",
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

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                }.AsQueryable());

            var dealerCars = this.mockService.GetAllDealershipCars(1, "userId", "id", null, 12);
            Assert.Single(dealerCars);
        }

        [Fact]
        public void GetAllDealershipCarsShouldReturnCorrectllySingleCarOnSecondPage()
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
                         Year = "1988",
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
                         Id = 23,
                         Make = new Make { Name = "bmw", },
                         Model = new Model { Id = 1, Name = "e60" },
                         Modification = "3.0",
                         Year = "2006",
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

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                }.AsQueryable());

            var dealerCars = this.mockService.GetAllDealershipCars(1, "userId", "id", string.Empty, 1);
            Assert.Single(dealerCars);
        }

        [Fact]
        public void GetAllDealershipCarsShouldSetCarValuesCorrectly()
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
                         Year = "1988",
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

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                }.AsQueryable());

            var dealerCars = this.mockService.GetAllDealershipCars(1, "userId", "id", null, 12).FirstOrDefault();
            Assert.Equal(23, dealerCars.Id);
            Assert.Equal("vw", dealerCars.MakeName);
            Assert.Equal("golf", dealerCars.ModelName);
            Assert.Equal("1.3", dealerCars.Modification);
            Assert.Equal("1988", dealerCars.Year);
            Assert.Equal("krg", dealerCars.Location);
            Assert.Equal(10, dealerCars.Mileage);
            Assert.Equal("white", dealerCars.ColorName);
            Assert.Equal("088", dealerCars.UserPhoneNumber);
            Assert.Equal(this.date, dealerCars.CreatedOn);
            Assert.Equal(500, dealerCars.Price);
            Assert.Equal("BGN", dealerCars.Currency);
            Assert.Equal("desc", dealerCars.Description);
            Assert.Equal("/images/cars/picId.ext", dealerCars.PictureUrl);
            Assert.Equal("vw golf 1.3...", dealerCars.CarTitle);
            Assert.False(dealerCars.IsInWatchlist);
        }

        [Fact]
        public void GetAllShouldReturnCorrectllyOneDealership()
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
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                }.AsQueryable());

            var dealerCars = this.mockService.GetAll<DealershipInListViewModel>(1, 12);
            Assert.Single(dealerCars);
        }

        [Fact]
        public void GetAllShouldReturnCorrectllyOneDealershipOnPageTwo()
        {
            this.mockUserRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<ApplicationUser>()
                {
                    new ApplicationUser
                    {
                        Id = "userId",
                        PhoneNumber = "088",
                    },
                    new ApplicationUser
                    {
                        Id = "userId2",
                        PhoneNumber = "0882",
                    },
                }.AsQueryable());

            this.mockDealershipRepository.Setup(x => x.AllAsNoTracking())
                .Returns(new List<Dealership>()
                {
                    new Dealership
                    {
                        Id = "id",
                        Name = "autoa",
                        Location = "Sofiq",
                        PhoneNumber = "000",
                        UserId = "userId",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId", Extension = "ext", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test",
                        DealerSince = this.date,
                        LogoPictureId = "picId",
                    },
                    new Dealership
                    {
                        Id = "id2",
                        Name = "autoa2",
                        Location = "Sofiq2",
                        PhoneNumber = "0002",
                        UserId = "userId2",
                        CreatedOn = this.date,
                        LogoPicture = new Picture { Id = "picId2", Extension = "ext2", },
                        Votes = new List<Vote>() { new Vote { Value = 5 }, new Vote { Value = 5 } },
                        Description = "test2",
                        DealerSince = this.date,
                        LogoPictureId = "picId2",
                    },
                }.AsQueryable());

            var dealerCars = this.mockService.GetAll<DealershipInListViewModel>(2, 1);
            Assert.Single(dealerCars);
        }
    }
}
