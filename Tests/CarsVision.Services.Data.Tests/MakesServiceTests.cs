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
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class MakesServiceTests
    {
        private readonly MakesService service;
        private readonly ApplicationDbContext dbContext;
        private readonly IDeletableEntityRepository<Make> makesRepository;

        public MakesServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
            this.dbContext = new ApplicationDbContext(options);

            this.makesRepository = new EfDeletableEntityRepository<Make>(this.dbContext);
            this.service = new MakesService(this.makesRepository);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public async Task GetAllNamesShouldReturnAll()
        {
            var make1 = new Make() { Id = 1, Name = "Vw", };
            var make2 = new Make() { Id = 2, Name = "audi", };
            var make3 = new Make() { Id = 3, Name = "bmw", };

            this.dbContext.Add(make1);
            this.dbContext.Add(make2);
            this.dbContext.Add(make3);
            await this.dbContext.SaveChangesAsync();

            var result = this.service.GetAllNames<MakesViewModel>();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void GetAllNamesShouldReturnZero()
        {
            var result = this.service.GetAllNames<MakesViewModel>();
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetMakeModelsShouldReturnAll()
        {
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

            this.dbContext.Add(make);
            await this.dbContext.SaveChangesAsync();

            var result = this.service.GetMakeModels(make.Name);

            Assert.Equal("golf", result.First());
        }

        [Fact]
        public async Task GetMakeModelsShouldReturnZero()
        {
            var make = new Make()
            {
                Id = 1,
                Name = "Vw",
                Models = new List<Model>(),
            };

            this.dbContext.Add(make);
            await this.dbContext.SaveChangesAsync();

            var result = this.service.GetMakeModels(make.Name);

            Assert.Empty(result);
        }
    }
}
