namespace CarsVision.Services.Data
{
    using System.Collections.Generic;

    public interface IColorsService
    {
        IEnumerable<T> GetAll<T>();
    }
}
