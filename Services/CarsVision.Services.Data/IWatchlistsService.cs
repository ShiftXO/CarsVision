namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CarsVision.Web.ViewModels.Cars;

    public interface IWatchlistsService
    {
        Task Add(int id, string userId);

        Task Remove(int id, string userId);

        int GetCount(string userId);

        bool IsInWatchlist(int id, string userId);

        IEnumerable<CarInListViewModel> GetAll(int page, int itemsPerPage, string userId);
    }
}
