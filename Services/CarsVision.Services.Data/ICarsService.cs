namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;

    public interface ICarsService
    {
        Task AddCarAsync(CarInputModel carInput, string userId);

        IEnumerable<T> GetAll<T>();

        IEnumerable<T> SearchCars<T>(CarsSearchInputModel car);
    }
}
