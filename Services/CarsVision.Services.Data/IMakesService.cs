namespace CarsVision.Services.Data
{
    using System.Collections.Generic;

    public interface IMakesService
    {
        IEnumerable<T> GetAllNames<T>();

        ICollection<string> GetMakeModels(string makeName);
    }
}
