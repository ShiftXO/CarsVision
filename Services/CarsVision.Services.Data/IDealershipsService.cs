namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Dealerships;

    public interface IDealershipsService
    {
        Task<bool> CreateDealershipAsync(CreateDealershipInputModel input, ApplicationUser user, string picturePath);

        IEnumerable<T> GetAll<T>(int page, int itemsPerPage);

        IEnumerable<CarInListViewModel> GetAllDealershipCars(int page, string dealershipId, int itemsPerPage);

        T GetById<T>(string id);

        int GetCount();

        int GetDealershipsCarsCount(string dealershipId);

        DealershipInfoViewModel GetDealershipInfo(string id);
    }
}
