namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Web.ViewModels.Cars;
    using Microsoft.EntityFrameworkCore;

    public class WatchlistsService : IWatchlistsService
    {
        private readonly IRepository<Watchlist> watchlistRepository;

        public WatchlistsService(
            IRepository<Watchlist> watchlistRepository)
        {
            this.watchlistRepository = watchlistRepository;
        }

        public async Task Add(int carId, string userId)
        {
            var wl = this.watchlistRepository.All().FirstOrDefault(x => x.CarId == carId && x.UserId == userId);

            if (wl == null)
            {
                wl = new Watchlist
                {
                    CarId = carId,
                    UserId = userId,
                };
            }

            await this.watchlistRepository.AddAsync(wl);
            await this.watchlistRepository.SaveChangesAsync();
        }

        public async Task Remove(int carId, string userId)
        {
            var car = this.watchlistRepository.All().FirstOrDefault(x => x.CarId == carId && x.UserId == userId);
            this.watchlistRepository.Delete(car);
            await this.watchlistRepository.SaveChangesAsync();
        }

        public IEnumerable<CarInListViewModel> GetAll(int page, int itemsPerPage, string userId)
        {
            var t = this.watchlistRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId).FirstOrDefault();
            var cars = this.watchlistRepository.AllAsNoTracking()
                .Where(x => x.UserId == userId)
                .Select(x => new CarInListViewModel
                {
                    Id = x.Car.Id,
                    IsInWatchlist = true,
                    MakeName = x.Car.Make.Name,
                    ModelName = x.Car.Model.Name,
                    Modification = x.Car.Modification,
                    Year = x.Car.Year,
                    Location = x.Car.Location,
                    Mileage = x.Car.Mileage,
                    ColorName = x.Car.Color.Name,
                    UserPhoneNumber = x.User.PhoneNumber,
                    CreatedOn = x.CreatedOn,
                    Price = x.Car.Price,
                    Currency = x.Car.Currency.ToString(),
                    Description = x.Car.Description,
                    PictureUrl = x.Car.ImageUrl != null ? x.Car.ImageUrl : "/images/cars/" + x.Car.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Car.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension,
                })
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .ToList();

            return cars;
        }

        public int GetCount(string userId)
        {
            return this.watchlistRepository.AllAsNoTracking().Where(x => x.UserId == userId).Count();
        }

        public bool IsInWatchlist(int carId, string userId)
        {
            return this.watchlistRepository.AllAsNoTracking().Any(x => x.CarId == carId && x.UserId == userId);
        }
    }
}
