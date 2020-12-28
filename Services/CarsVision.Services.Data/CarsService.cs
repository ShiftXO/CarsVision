namespace CarsVision.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Colors;
    using CarsVision.Web.ViewModels.Extras;
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
        private readonly IRepository<Extra> extrasRepository;
        private readonly ICommonService commonService;

        public CarsService(
            IDeletableEntityRepository<Car> carRepository,
            IDeletableEntityRepository<Make> makeRepository,
            IRepository<Color> colorRepository,
            IDeletableEntityRepository<Dealership> dealershipRepository,
            IDeletableEntityRepository<ApplicationUser> userRepository,
            IRepository<Watchlist> watchlistRepository,
            IRepository<Extra> extrasRepository,
            ICommonService commonService)
        {
            this.carRepository = carRepository;
            this.makeRepository = makeRepository;
            this.colorRepository = colorRepository;
            this.dealershipRepository = dealershipRepository;
            this.userRepository = userRepository;
            this.watchlistRepository = watchlistRepository;
            this.extrasRepository = extrasRepository;
            this.commonService = commonService;
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

            var color = this.colorRepository.All()
                .Select(x => new { x.Id, x.Name })
                .FirstOrDefault(x => x.Name == input.Color);

            var car = new Car
            {
                MakeId = make.Id,
                ModelId = modelId,
                UserId = userId,
                Modification = input.Modification,
                Price = input.Price >= 300 ? input.Price : 0,
                Power = input.Power,
                Month = input.Month,
                Views = 0,
                Condition = input.Condition,
                Category = input.Category,
                Year = input.Year,
                Mileage = input.Mileage,
                IsVIP = input.IsVIP,
                Location = input.Location,
                Description = input.Description,
                EuroStandard = input.EuroStandard,
                Currency = input.Currency,
                Gearbox = input.Gearbox,
                EngineType = input.EngineType,
                ColorId = color.Id,
                Validity = input.Validity,
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

            if (input.Extras != null)
            {
                foreach (var extra in input.Extras)
                {
                    car.Extras.Add(new CarsExtras { CarId = car.Id, ExtraId = extra });
                }
            }

            await this.carRepository.AddAsync(car);
            await this.carRepository.SaveChangesAsync();
        }

        public IEnumerable<CarInListViewModel> GetAll(int page, string userId, string order, int itemsPerPage = 12)
        {
            var query = this.carRepository.AllAsNoTracking()
                .Select(x => new CarInListViewModel
                {
                    Id = x.Id,
                    IsInWatchlist = userId == string.Empty ? false : this.watchlistRepository.All().Any(d => d.UserId == userId && d.CarId == x.Id),
                    MakeName = x.Make.Name,
                    ModelName = x.Model.Name,
                    Modification = x.Modification,
                    Year = x.Year,
                    Location = x.Location,
                    Mileage = x.Mileage,
                    ColorName = x.Color.Name,
                    UserPhoneNumber = x.User.PhoneNumber,
                    Currency = x.Currency.ToString(),
                    CreatedOn = x.CreatedOn,
                    Price = x.Price,
                    PriceOrder = x.Currency == Currency.EUR ? (x.Price * 1.96M) : x.Currency == Currency.USD ? (x.Price * 1.61M) : x.Price,
                    Description = x.Description,
                    PictureUrl = x.ImageUrl != null ? x.ImageUrl : "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .AsQueryable();

            var orderModel = new CarsSearchInputModel { Order = order };
            query = this.commonService.Filter(query, orderModel, page, itemsPerPage);
            return query.Take(itemsPerPage).ToList();
        }

        public CarPostViewModel GetAllMakesAndColors()
        {
            var model = new CarPostViewModel
            {
                Makes = this.makeRepository.AllAsNoTracking().Select(x => new MakesViewModel { Name = x.Name }).OrderBy(x => x.Name).ToList(),
                Colors = this.colorRepository.AllAsNoTracking().Select(x => new ColorsViewModel { Name = x.Name }).OrderBy(x => x.Name).ToList(),
                Extras = this.extrasRepository.AllAsNoTracking().Select(x => new ExtrasViewModel { Id = x.Id, Name = x.Name }).ToList(),
            };

            return model;
        }

        public IEnumerable<CarInListViewModel> GetNewest()
        {
            var query = this.carRepository.AllAsNoTracking()
                .Select(x => new CarInListViewModel
                {
                    Id = x.Id,
                    MakeName = x.Make.Name,
                    ModelName = x.Model.Name,
                    Modification = x.Modification,
                    Location = x.Location,
                    Mileage = x.Mileage,
                    Currency = x.Currency.ToString(),
                    CreatedOn = x.CreatedOn,
                    Price = x.Price,
                    PictureUrl = x.ImageUrl != null ? x.ImageUrl : "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .OrderByDescending(x => x.CreatedOn)
                .Take(8)
                .AsQueryable();

            return query.ToList();
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
                    Month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(x.Month),
                    Currency = x.Currency,
                    EngineType = x.EngineType,
                    EuroStandard = x.EuroStandard,
                    Gearbox = x.Gearbox,
                    Mileage = x.Mileage,
                    Power = x.Power != null ? (int)x.Power : 0,
                    Price = x.Price,
                    Views = x.Views,
                    PictureUrls = pictures,
                    Extras = x.Extras.Select(x => x.Extra.Name).ToList(),
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

        public (IEnumerable<CarInListViewModel> Cars, int Count) SearchCars(CarsSearchInputModel car, string userId, int page, int itemsPerPage)
        {
            var query = this.carRepository.AllAsNoTracking()
                .Select(x => new CarInListViewModel
                {
                    Id = x.Id,
                    IsInWatchlist = userId == string.Empty ? false : this.watchlistRepository.All().Any(d => d.UserId == userId && d.CarId == x.Id),
                    MakeName = x.Make.Name,
                    ModelName = x.Model.Name,
                    Modification = x.Modification,
                    Year = x.Year,
                    Location = x.Location,
                    Mileage = x.Mileage,
                    ColorName = x.Color.Name,
                    UserPhoneNumber = x.User.PhoneNumber,
                    EngineType = x.EngineType,
                    Gearbox = x.Gearbox,
                    Currency = x.Currency.ToString(),
                    CreatedOn = x.CreatedOn,
                    Price = x.Price,
                    PriceOrder = x.Currency == Currency.EUR ? (x.Price * 1.96M) : x.Currency == Currency.USD ? (x.Price * 1.61M) : x.Price,
                    Description = x.Description,
                    PictureUrl = x.ImageUrl != null ? x.ImageUrl : "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .AsQueryable();

            query = this.commonService.Filter(query, car, page, itemsPerPage);

            return (query.Take(itemsPerPage).ToList(), query.ToList().Count);
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
            car.ColorId = colorId > 0 ? colorId : null;
            car.Price = input.Price;
            car.EngineType = input.EngineType;
            car.Gearbox = input.Gearbox;
            car.Year = int.Parse(input.Year);
            car.Modification = input.Modification;
            car.Currency = input.Currency;
            car.Description = input.Description;
            car.Power = input.Power;
            car.Mileage = input.Mileage;
            car.Location = input.Location;

            await this.carRepository.SaveChangesAsync();
        }

        public async Task Delete(int id, string userId)
        {
            var car = this.carRepository.All().FirstOrDefault(x => x.Id == id && x.UserId == userId);
            if (car != null)
            {
                this.carRepository.Delete(car);
                await this.carRepository.SaveChangesAsync();
            }
        }

        public async Task<int> IncreaseViews(int carId)
        {
            var car = this.carRepository.All().FirstOrDefault(x => x.Id == carId);
            car.Views++;
            await this.carRepository.SaveChangesAsync();
            return car.Views;
        }
    }
}
