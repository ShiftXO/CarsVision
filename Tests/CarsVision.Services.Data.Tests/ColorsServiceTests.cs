namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Colors;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ColorsServiceTests
    {
        private readonly ColorsService service;
        private readonly ApplicationDbContext dbContext;
        private readonly IRepository<Color> colorsRepository;

        public ColorsServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);

            this.colorsRepository = new EfRepository<Color>(this.dbContext);
            this.service = new ColorsService(this.colorsRepository);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public async Task GetAllColorsShouldReturnAll()
        {
            // https://www.youtube.com/watch?v=5W6fE8aGJF4&ab_channel=TheGrumpySounds
            var color1 = new Color() { Id = 1, Name = "white", };
            var color2 = new Color() { Id = 2, Name = "green", };
            var color3 = new Color() { Id = 3, Name = "red", };

            this.dbContext.Add(color1);
            this.dbContext.Add(color2);
            this.dbContext.Add(color3);
            await this.dbContext.SaveChangesAsync();

            var result = this.service.GetAll<ColorsViewModel>();
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void GetAllColorsShouldReturnZero()
        {
            var result = this.service.GetAll<ColorsViewModel>();
            Assert.Empty(result);
        }
    }
}
