namespace CarsVision.Services.Data.Tests
{
    using System.Reflection;

    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using Xunit;

    public class MLServiceTests
    {
        private readonly IMLService service;

        public MLServiceTests()
        {
            this.service = new MLService();
            AutoMapperConfig.RegisterMappings(Assembly.Load("CarsVision.Web.ViewModels"));
        }

        [Fact]
        public void GetValuesShouldReturnCorrectlly()
        {
            var car = new CarPredictInputModel
            {
                Make = "vw",
                Model = "golf",
                Power = 55,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Manual,
                Price = 500,
                Year = "1988",
                Mileage = 110,
            };

            var result = this.service.GetValues(car);
            Assert.Equal("vw", result.Make);
            Assert.Equal("golf", result.Model);
            Assert.Equal(55, result.Power);
            Assert.Equal("EURO 3", result.Eurostandard);
            Assert.Equal("Ръчни скорости", result.Gearbox);
            Assert.Equal(1988, result.Year);
            Assert.Equal(110, result.Mileage);
        }

        [Fact]
        public void GetValuesShouldSetGearboxToAutomatic()
        {
            var car = new CarPredictInputModel
            {
                Make = "vw",
                Model = "golf",
                Power = 55,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Automatic,
                Price = 500,
                Year = "1988",
                Mileage = 110,
            };

            var result = this.service.GetValues(car);
            Assert.Equal("vw", result.Make);
            Assert.Equal("golf", result.Model);
            Assert.Equal(55, result.Power);
            Assert.Equal("EURO 3", result.Eurostandard);
            Assert.Equal("Автоматични скорости", result.Gearbox);
            Assert.Equal(1988, result.Year);
            Assert.Equal(110, result.Mileage);
        }

        [Fact]
        public void GetValuesShouldSetGearboxToAutomaticWhenSemiAutomatic()
        {
            var car = new CarPredictInputModel
            {
                Make = "vw",
                Model = "golf",
                Power = 55,
                EuroStandard = EuroStandard.Euro_3,
                Gearbox = Gearbox.Semi_Automatic,
                Price = 500,
                Year = "1988",
                Mileage = 110,
            };

            var result = this.service.GetValues(car);
            Assert.Equal("vw", result.Make);
            Assert.Equal("golf", result.Model);
            Assert.Equal(55, result.Power);
            Assert.Equal("EURO 3", result.Eurostandard);
            Assert.Equal("Автоматични скорости", result.Gearbox);
            Assert.Equal(1988, result.Year);
            Assert.Equal(110, result.Mileage);
        }
    }
}
