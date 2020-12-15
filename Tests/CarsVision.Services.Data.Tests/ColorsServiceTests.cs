namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Colors;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class ColorsServiceTests
    {
        public ColorsServiceTests()
        {
            this.InitializeMapper();
        }

        [Fact]
        public async Task GetAllColorsShouldReturnAll()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfRepository<Color>(dbContext);

            // https://www.youtube.com/watch?v=5W6fE8aGJF4&ab_channel=TheGrumpySounds
            var service = new ColorsService(repository);
            var color1 = new Color() { Id = 1, Name = "white", };
            var color2 = new Color() { Id = 2, Name = "green", };
            var color3 = new Color() { Id = 3, Name = "red", };

            dbContext.Add(color1);
            dbContext.Add(color2);
            dbContext.Add(color3);
            await dbContext.SaveChangesAsync();

            var result = service.GetAll<ColorsViewModel>();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void GetAllColorsShouldReturnZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfRepository<Color>(dbContext);

            var service = new ColorsService(repository);

            var result = service.GetAll<ColorsViewModel>();

            Assert.Empty(result);
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
    }
}
