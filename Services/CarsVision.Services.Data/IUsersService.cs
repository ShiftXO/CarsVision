namespace CarsVision.Services.Data
{
    using System.Collections.Generic;

    using System.Threading.Tasks;

    using CarsVision.Web.ViewModels.Cars;

    public interface IUsersService
    {
        Task<IEnumerable<CarInListViewModel>> GetAll(int page, int itemsPerPage, string userId);

        int GetCount(string userId);

        CarEditViewModel GetCarById(int carId);

        Task<string> AddCarToWatchlist(int id, string userId);
    }
}
