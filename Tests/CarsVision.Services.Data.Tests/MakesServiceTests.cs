namespace CarsVision.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CarsVision.Data;
    using CarsVision.Data.Models;
    using CarsVision.Data.Repositories;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class MakesServiceTests
    {
        public MakesServiceTests()
        {
            this.InitializeMapper();
        }

        [Fact]
        public async Task GetAllNamesShouldReturnAll()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Make>(dbContext);

            var service = new MakesService(repository);
            var make1 = new Make() { Id = 1, Name = "Vw", };
            var make2 = new Make() { Id = 2, Name = "audi", };
            var make3 = new Make() { Id = 3, Name = "bmw", };

            dbContext.Add(make1);
            dbContext.Add(make2);
            dbContext.Add(make3);
            await dbContext.SaveChangesAsync();

            var result = service.GetAllNames<MakesViewModel>();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void GetAllNamesShouldReturnZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Make>(dbContext);

            var service = new MakesService(repository);

            var result = service.GetAllNames<MakesViewModel>();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMakeModelsShouldReturnAll()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Make>(dbContext);

            var service = new MakesService(repository);
            var make = new Make()
            {
                Id = 1,
                Name = "Vw",
                Models = new List<Model>
                {
                    new Model
                    {
                        MakeId = 1,
                        Name = "golf",
                    },
                },
            };

            dbContext.Add(make);
            await dbContext.SaveChangesAsync();

            var result = service.GetMakeModels(make.Name);

            Assert.Equal("golf", result.First());
        }

        [Fact]
        public async Task GetMakeModelsShouldReturnZero()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            var dbContext = new ApplicationDbContext(options);

            var repository = new EfDeletableEntityRepository<Make>(dbContext);

            var service = new MakesService(repository);
            var make = new Make()
            {
                Id = 1,
                Name = "Vw",
                Models = new List<Model>(),
            };

            dbContext.Add(make);
            await dbContext.SaveChangesAsync();

            var result = service.GetMakeModels(make.Name);

            Assert.Empty(result);
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
    }
}
