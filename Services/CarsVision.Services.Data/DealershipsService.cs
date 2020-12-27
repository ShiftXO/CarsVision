namespace CarsVision.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Common;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Dealerships;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.EntityFrameworkCore;

    public class DealershipsService : IDealershipsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png" };

        private readonly IDeletableEntityRepository<Dealership> dealershipRepository;
        private readonly IDeletableEntityRepository<Car> carsRepository;
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;
        private readonly IRepository<Watchlist> watchlistRepository;
        private readonly ICommonService commonService;

        public DealershipsService(
            IDeletableEntityRepository<Dealership> dealershipRepository,
            IDeletableEntityRepository<Car> carsRepository,
            IDeletableEntityRepository<ApplicationUser> usersRepository,
            IRepository<Watchlist> watchlistRepository,
            ICommonService commonService)
        {
            this.dealershipRepository = dealershipRepository;
            this.carsRepository = carsRepository;
            this.usersRepository = usersRepository;
            this.watchlistRepository = watchlistRepository;
            this.commonService = commonService;
        }

        public async Task<bool> CreateDealershipAsync(CreateDealershipInputModel input, ApplicationUser user, string picturePath)
        {
            var dealership = new Dealership
            {
                Name = input.DealershipName,
                DealerSince = DateTime.UtcNow,
                Location = input.Location + " " + input.FullAddress,
                PhoneNumber = input.PhoneNumber,
                Description = input.Description,
                Stars = 0,
                User = user,
                UserId = user.Id,
            };

            // /wwwroot/images/dealerships/jhdsi-343g3h453-=g34g.jpg
            Directory.CreateDirectory($"{picturePath}/dealerships/");
            var extension = Path.GetExtension(input.LogoPicture.FileName).TrimStart('.');
            if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
            {
                throw new Exception($"Invalid picture extension {extension}");
            }

            var dbPicture = new Picture
            {
                Extension = extension,
            };
            dealership.LogoPicture = dbPicture;

            var physicalPath = $"{picturePath}/dealerships/{dbPicture.Id}.{extension}";
            using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
            await input.LogoPicture.CopyToAsync(fileStream);

            if (dealership != null)
            {
                await this.dealershipRepository.AddAsync(dealership);
                await this.dealershipRepository.SaveChangesAsync();

                return true;
            }

            throw new InvalidOperationException(GlobalConstants.InvalidOperationExceptionWhileCreatingDealership);
        }

        public IEnumerable<T> GetAll<T>(int page, int itemsPerPage)
        {
            var dealerships = this.dealershipRepository.AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .To<T>().ToList();
            return dealerships;
        }

        public IEnumerable<CarInListViewModel> GetAllDealershipCars(int page, string dealershipId, string userId, string order, int itemsPerPage)
        {
            var dbCar = this.carsRepository.AllAsNoTracking().Where(x => x.UserId == dealershipId);
            var query = dbCar
                .Select(x => new CarInListViewModel
                {
                    Id = x.Id,
                    IsInWatchlist = userId == string.Empty ? false : this.watchlistRepository.All().Any(d => d.UserId == userId && d.CarId == x.Id),
                    MakeName = x.Make.Name,
                    ModelName = x.Model.Name,
                    Modification = x.Modification,
                    Year = x.Year.ToString(),
                    Location = x.Location,
                    Mileage = (int)x.Mileage,
                    ColorName = x.Color.Name,
                    UserPhoneNumber = x.User.PhoneNumber,
                    CreatedOn = x.CreatedOn,
                    Price = (decimal)x.Price,
                    PriceOrder = x.Currency == Currency.EUR ? ((decimal)x.Price * 1.96M) : x.Currency == Currency.USD ? ((decimal)x.Price * 1.61M) : (decimal)x.Price,
                    Currency = x.Currency.ToString(),
                    Description = x.Description,
                    PictureUrl = x.ImageUrl != null ? x.ImageUrl : "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .AsQueryable();

            var orderModel = new CarsSearchInputModel { Order = order };

            query = this.commonService.Filter(query, orderModel, page, itemsPerPage);

            return query.Take(itemsPerPage).ToList();
        }

        public int GetCount()
        {
            return this.dealershipRepository.AllAsNoTracking().Count();
        }

        public DealershipInfoViewModel GetDealershipInfo(string id)
        {
            var dealership = this.dealershipRepository.AllAsNoTracking()
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Location,
                    x.CreatedOn,
                    x.LogoPicture,
                    x.PhoneNumber,
                    x.UserId,
                    LogoPictureId = x.LogoPicture.Id,
                    LogoPictureExtension = x.LogoPicture.Extension,
                    AverageVote = !x.Votes.Any() ? 0 : x.Votes.Average(x => x.Value),
                })
                .FirstOrDefault(x => x.UserId == id);

            var res = new DealershipInfoViewModel
            {
                Id = dealership.Id,
                UserId = dealership.UserId,
                Name = dealership.Name,
                Location = dealership.Location,
                CreatedOn = dealership.CreatedOn,
                LogoPicture = "/images/dealerships/" + dealership.LogoPictureId + "." + dealership.LogoPictureExtension,
                PhoneNumber = dealership.PhoneNumber,
                AverageVote = dealership.AverageVote,
            };

            return res;
        }

        public int GetDealershipsCarsCount(string id)
        {
            return this.usersRepository.AllAsNoTracking().Where(x => x.Id == id).Select(x => x.Cars).FirstOrDefault().Count();
        }
    }
}
