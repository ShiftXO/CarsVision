namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;

    public interface ICarsService
    {
        Task AddCarAsync(CreateCarInputModel carInput, string userId, string picturePath);

        IEnumerable<CarInListViewModel> GetAll(int page, string userId, string order, int itemsPerPage);

        (IEnumerable<CarInListViewModel> Cars, int Count) SearchCars(CarsSearchInputModel car, string userId, int page, int itemsPerPage);

        int GetCount();

        SingleCarViewModel GetById(int id);

        CarPostViewModel GetAllMakesAndColors();

        Task Update(CarEditViewModel car);

        Task Delete(int id);

        Task<int> IncreaseViews(int carId);
    }
}
