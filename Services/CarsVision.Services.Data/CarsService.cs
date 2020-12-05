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
    using Microsoft.EntityFrameworkCore;

    public class CarsService : ICarsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png" };

        private readonly IDeletableEntityRepository<Car> carRepository;
        private readonly IDeletableEntityRepository<Make> makeRepository;
        private readonly IRepository<Color> colorRepository;
        private readonly IDeletableEntityRepository<Dealership> dealershipRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> userRepository;
        private readonly IRepository<Watchlist> watchlistRepository;

        public CarsService(
            IDeletableEntityRepository<Car> carRepository,
            IDeletableEntityRepository<Make> makeRepository,
            IRepository<Color> colorRepository,
            IDeletableEntityRepository<Dealership> dealershipRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IRepository<Watchlist> watchlistRepository)
        {
            this.carRepository = carRepository;
            this.makeRepository = makeRepository;
            this.colorRepository = colorRepository;
            this.dealershipRepository = dealershipRepository;
            this.userRepository = userRepository;
            this.watchlistRepository = watchlistRepository;
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

        public async Task<IEnumerable<CarInListViewModel>> GetAll(int page, string userId, int itemsPerPage = 12)
        {
            var cars = await this.carRepository.AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new CarInListViewModel
                {
                    Id = x.Id,
                    IsInWatchlist = userId == string.Empty ? false : this.watchlistRepository.All().Any(d => d.UserId == userId && d.CarId == x.Id),
                    MakeName = x.Make.Name,
                    ModelName = x.Model.Name,
                    Modification = x.Modification,
                    Year = x.Year,
                    Location = x.Location,
                    Mileage = (int)x.Mileage,
                    ColorName = x.Color.Name,
                    UserPhoneNumber = x.User.PhoneNumber,
                    Currency = x.Currency.ToString(),
                    CreatedOn = x.CreatedOn,
                    Price = (decimal)x.Price,
                    Description = x.Description,
                    PictureUrl = x.ImageUrl != null ? x.ImageUrl : "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .ToListAsync();

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

        public SingleCarViewModel GetById(int id)
        {
            var dbCar = this.carRepository.AllAsNoTracking().Select(x => new { x.Id, x.UserId, x.Pictures, x.Make.Name, Model = x.Model.Name }).FirstOrDefault(x => x.Id == id);
            var dealership = this.dealershipRepository.AllAsNoTracking().Where(x => x.UserId == dbCar.UserId).FirstOrDefault();
            var user = this.userRepository.AllAsNoTracking().Where(x => x.Id == dbCar.UserId).FirstOrDefault();

            var pictures = dbCar.Pictures.Select(x => x.Id + "." + x.Extension).ToList();

            var car = this.carRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new SingleCarViewModel
                {
                    Id = x.Id,
                    MakeName = dbCar.Name,
                    ModelName = dbCar.Model,
                    Modification = x.Modification,
                    ColorName = x.Color.Name,
                    CreatedOn = x.CreatedOn,
                    Location = x.Location,
                    Description = x.Description,
                    Year = x.Year,
                    Currency = (Currency)x.Currency,
                    EngineType = (EngineType)x.EngineType,
                    EuroStandard = (EuroStandard)x.EuroStandard,
                    Gearbox = (Gearbox)x.Gearbox,
                    Mileage = x.Mileage != null ? (int)x.Mileage : 0,
                    Power = x.Power != null ? (int)x.Power : 0,
                    Price = x.Price != null ? (decimal)x.Price : 0,
                    Views = x.Views != null ? (int)x.Views : 0,
                    PictureUrls = pictures,
                })
                .FirstOrDefault();

            if (dealership == null)
            {
                car.PhoneNumber = user.PhoneNumber;
                car.UserId = user.Id;
                car.IsDealership = false;
            }
            else
            {
                car.PhoneNumber = dealership.PhoneNumber;
                car.DealershipName = dealership.Name;
                car.UserId = dealership.UserId;
                car.IsDealership = true;
            }

            return car;
        }

        public int GetCount()
        {
            return this.carRepository.AllAsNoTracking().Count();
        }

        public (IEnumerable<T> Cars, int Count) SearchCars<T>(CarsSearchInputModel car, int page, int itemsPerPage)
        {
            var query = this.carRepository.AllAsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(car.Make))
            {
                query = query.Where(x => x.Make.Name == car.Make);
            }

            if (!string.IsNullOrWhiteSpace(car.Model))
            {
                query = query.Where(x => x.Model.Name == car.Model);
            }

            if (car.EngineType != EngineType.Unknown)
            {
                query = query.Where(x => x.EngineType == car.EngineType);
            }

            if (car.Gearbox != Gearbox.None)
            {
                query = query.Where(x => x.Gearbox == car.Gearbox);
            }

            if (car.Price > 0)
            {
                query = query.Where(x => x.Price <= car.Price);
            }

            if (car.Year > 0)
            {
                query = query.Where(x => x.Year.Contains(car.Year.ToString()));
            }

            query.OrderByDescending(x => x.CreatedOn)
            .Skip((page - 1) * itemsPerPage);

            return (query.To<T>().Take(itemsPerPage).ToList(), query.To<T>().ToList().Count);
        }

        public async Task Update(CarEditViewModel input)
        {
            var car = this.carRepository.All().FirstOrDefault(x => x.Id == input.Id);

            var make = this.makeRepository.All()
                .Where(x => x.Name == input.Make)
                .Select(x => new { x.Id, x.Models })
                .FirstOrDefault();

            var modelId = make.Models
                .Where(x => x.Name == input.Model)
                .Select(x => x.Id)
                .FirstOrDefault();

            var colorId = this.colorRepository.All()
                .Where(x => x.Name == input.Color)
                .Select(x => x.Id)
                .FirstOrDefault();

            car.MakeId = make.Id;
            car.ModelId = modelId;
            car.ColorId = colorId;
            car.Price = input.Price;
            car.EngineType = input.EngineType;
            car.Gearbox = input.Gearbox;
            car.Year = input.Month + " " + input.Year;
            car.Modification = input.Modification;
            car.Currency = input.Currency;
            car.Description = input.Description;
            car.Power = input.Power;
            car.Mileage = input.Mileage;

            await this.carRepository.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            var car = this.carRepository.All().FirstOrDefault(x => x.Id == id);
            this.carRepository.Delete(car);
            await this.carRepository.SaveChangesAsync();
        }
    }
}
