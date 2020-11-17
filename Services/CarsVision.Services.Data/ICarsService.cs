namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CarsVision.Web.ViewModels.Cars;

    public interface ICarsService
    {
        Task AddCarAsync(CarInputModel carInput, string userId);

        IEnumerable<T> GetAll<T>();
    }
}
