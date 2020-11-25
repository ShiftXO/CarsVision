namespace CarsVision.Services.Data
{
    using System.Collections.Generic;

    public interface IDealershipsService
    {
        IEnumerable<T> GetAll<T>(int page, int itemsPerPage);

        T GetById<T>(string id);

        int GetCount();
    }
}
