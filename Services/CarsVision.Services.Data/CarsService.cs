﻿namespace CarsVision.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;

    public class CarsService : ICarsService
    {
        private readonly IDeletableEntityRepository<Car> carRepository;
        private readonly IDeletableEntityRepository<Make> makeRepository;
        private readonly IRepository<Color> colorRepository;

        public CarsService(
            IDeletableEntityRepository<Car> carRepository,
            IDeletableEntityRepository<Make> makeRepository,
            IRepository<Color> colorRepository)
        {
            this.carRepository = carRepository;
            this.makeRepository = makeRepository;
            this.colorRepository = colorRepository;
        }

        public async Task AddCarAsync(CarInputModel carInput, string userId)
        {
            var make = this.makeRepository.All()
                .Where(x => x.Name == carInput.Make)
                .Select(x => new { x.Id, x.Models })
                .FirstOrDefault();

            var modelId = make.Models
                .Where(x => x.Name == carInput.Model)
                .Select(x => x.Id)
                .FirstOrDefault();

            var colorId = this.colorRepository.All()
                .Select(x => x.Id)
                .FirstOrDefault();

            var car = new Car
            {
                MakeId = make.Id,
                ModelId = modelId,
                UserId = userId,
                ImageUrl = carInput.ImageUrl,
                Modification = carInput.Modification,
                Price = carInput.Price,
                Power = carInput.Power,
                Year = carInput.Year.ToString(),
                Mileage = carInput.Mileage,
                IsVIP = carInput.IsVIP,
                Location = carInput.Location,
                Description = carInput.Description,
                EuroStandard = Enum.Parse<EuroStandard>(carInput.EuroStandard.ToString()),
                Currency = Enum.Parse<Currency>(carInput.Currency.ToString()),
                Gearbox = Enum.Parse<Gearbox>(carInput.Gearbox.ToString()),
                EngineType = Enum.Parse<EngineType>(carInput.EngineType.ToString()),
                ColorId = colorId,
            };

            await this.carRepository.AddAsync(car);
            await this.carRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>()
        {
            IQueryable<Car> query = this.carRepository.All();

            return query.To<T>().ToList();
        }

        public IEnumerable<T> SearchCars<T>(CarsSearchInputModel car)
        {
            IQueryable<Car> query = this.carRepository.AllAsNoTracking()
                .Where(x =>
                x.Make.Name == car.Make &&
                x.Model.Name == car.Model &&
                x.Price <= car.Price &&
                x.EngineType == car.EngineType &&
                x.Gearbox == car.Gearbox &&
                x.Year.Contains(car.Year.ToString()));

            return query.To<T>().ToList();
        }
    }
}
