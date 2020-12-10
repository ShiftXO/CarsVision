namespace CarsVision.Services.Data
{
    using CarsVision.Web.ViewModels.Cars;

    public interface IMLService
    {
        MLCarDTO GetValues(CarPredictInputModel input);
    }
}
