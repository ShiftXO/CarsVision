namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;

    public interface ICarsService
    {
        Task AddCarAsync(CreateCarInputModel carInput, string userId, string picturePath);

        IEnumerable<T> GetAll<T>(int page, int itemsPerPage);

        IEnumerable<T> SearchCars<T>(CarsSearchInputModel car, int page, int itemsPerPage);

        int GetCount();

        T GetById<T>(int id);

        CarPostViewModel GetAllMakesAndColors();

        Task Update(CarEditViewModel car);

        Task Delete(int id);
    }
}
