namespace CarsVision.Services.Data
{
    using System.Linq;

    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;

    public interface ICommonService
    {
        IQueryable<CarInListViewModel> Filter(IQueryable<CarInListViewModel> query, CarsSearchInputModel car, int page, int itemsPerPage);
    }
}
