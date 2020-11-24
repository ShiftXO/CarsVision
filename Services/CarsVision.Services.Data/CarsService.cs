namespace CarsVision.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Colors;
    using CarsVision.Web.ViewModels.Home;

    public class CarsService : ICarsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png" };

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

        public async Task AddCarAsync(CreateCarInputModel input, string userId, string picturePath)
        {
            var make = this.makeRepository.All()
                .Where(x => x.Name == input.Make)
                .Select(x => new { x.Id, x.Models })
                .FirstOrDefault();

            var modelId = make.Models
                .Where(x => x.Name == input.Model)
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
                Modification = input.Modification,
                Price = input.Price,
                Power = input.Power,
                Year = input.Month + " " + input.Year,
                Mileage = input.Mileage,
                IsVIP = input.IsVIP,
                Location = input.Location,
                Description = input.Description,
                EuroStandard = Enum.Parse<EuroStandard>(input.EuroStandard.ToString()),
                Currency = Enum.Parse<Currency>(input.Currency.ToString()),
                Gearbox = Enum.Parse<Gearbox>(input.Gearbox.ToString()),
                EngineType = Enum.Parse<EngineType>(input.EngineType.ToString()),
                ColorId = colorId,
            };

            // /wwwroot/images/cars/jhdsi-343g3h453-=g34g.jpg
            Directory.CreateDirectory($"{picturePath}/cars/");
            foreach (var picture in input.Pictures)
            {
                var extension = Path.GetExtension(picture.FileName).TrimStart('.');
                if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
                {
                    throw new Exception($"Invalid picture extension {extension}");
                }

                var dbPicture = new Picture
                {
                    Extension = extension,
                };
                car.Pictures.Add(dbPicture);

                var physicalPath = $"{picturePath}/cars/{dbPicture.Id}.{extension}";
                using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
                await picture.CopyToAsync(fileStream);
            }

            await this.carRepository.AddAsync(car);
            await this.carRepository.SaveChangesAsync();
        }

        public IEnumerable<T> GetAll<T>(int page, int itemsPerPage = 12)
        {
            var cars = this.carRepository.AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .To<T>().ToList();
            return cars;
        }

        public CarPostViewModel GetAllMakesAndColors()
        {
            var model = new CarPostViewModel
            {
                Makes = this.makeRepository.AllAsNoTracking().Select(x => new MakesViewModel { Name = x.Name }).ToList(),
                Colors = this.colorRepository.AllAsNoTracking().Select(x => new ColorsViewModel { Name = x.Name }).ToList(),
            };

            return model;
        }

        public T GetById<T>(int id)
        {
            var car = this.carRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

            return car;
        }

        public int GetCount()
        {
            return this.carRepository.AllAsNoTracking().Count();
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
