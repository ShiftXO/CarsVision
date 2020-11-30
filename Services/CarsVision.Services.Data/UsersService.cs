namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Colors;
    using CarsVision.Web.ViewModels.Home;

    public class UsersService : IUsersService
    {
        private readonly IDeletableEntityRepository<Car> carRepository;
        private readonly IRepository<Color> colorRepository;
        private readonly IDeletableEntityRepository<Make> makeRepository;

        public UsersService(
            IDeletableEntityRepository<Car> carRepository,
            IRepository<Color> colorRepository,
            IDeletableEntityRepository<Make> makeRepository)
        {
            this.carRepository = carRepository;
            this.colorRepository = colorRepository;
            this.makeRepository = makeRepository;
        }

        public IEnumerable<CarInListViewModel> GetAll<T>(int page, int itemsPerPage, string userId)
        {
            var cars = this.carRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .Select(x => new CarInListViewModel
                {
                    Id = x.Id,
                    MakeName = x.Make.Name,
                    ModelName = x.Model.Name,
                    CreatedOn = x.CreatedOn,
                    Price = (decimal)x.Price,
                    Modification = x.Modification,
                    Description = x.Description,
                    PictureUrl = x.ImageUrl != null ? x.ImageUrl : "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .ToList();
            return cars;
        }

        public CarEditViewModel GetCarById(int carId)
        {
            var car = this.carRepository.AllAsNoTracking().Where(x => x.Id == carId)
                .Select(x => new CarEditViewModel
                {
                    Make = x.Make.Name,
                    Model = x.Model.Name,
                    Modification = x.Modification,
                    Year = x.Year,
                    Gearbox = (Gearbox)x.Gearbox,
                    Currency = (Currency)x.Currency,
                    EngineType = (EngineType)x.EngineType,
                    EuroStandard = (EuroStandard)x.EuroStandard,
                    Location = x.Location,
                    Power = (int)x.Power,
                    Price = (int)x.Price,
                    Description = x.Description,
                    Makes = this.makeRepository.AllAsNoTracking().Select(x => new MakesViewModel { Name = x.Name }).ToList(),
                    Colors = this.colorRepository.AllAsNoTracking().Select(x => new ColorsViewModel { Name = x.Name }).ToList(),
                }).FirstOrDefault();

            return car;
        }

        public int GetCount(string userId)
        {
            return this.carRepository.AllAsNoTracking().Where(x => x.UserId == userId).Count();
        }
    }
}
