namespace CarsVision.Services.Data
{
    using System.Linq;

    using CarsVision.Data.Models;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;

    public class CommonService : ICommonService
    {
        public IQueryable<CarInListViewModel> Filter(IQueryable<CarInListViewModel> query, CarsSearchInputModel car, int page, int itemsPerPage)
        {
            if (!string.IsNullOrWhiteSpace(car.Make) && car.Make != "All")
            {
                query = query.Where(x => x.MakeName == car.Make);
            }

            if (!string.IsNullOrWhiteSpace(car.Model) && car.Model != "All")
            {
                query = query.Where(x => x.ModelName == car.Model);
            }

            if (car.EngineType != EngineType.Unknown)
            {
                query = query.Where(x => x.EngineType.ToString() == car.EngineType.ToString());
            }

            if (car.Gearbox != Gearbox.None)
            {
                query = query.Where(x => x.Gearbox == car.Gearbox);
            }

            if (car.MinPrice > 0 && car.MaxPrice > 0)
            {
                query = query.Where(x => x.Price >= car.MinPrice && x.Price <= car.MaxPrice);
            }
            else if (car.MinPrice > 0)
            {
                query = query.Where(x => x.Price >= car.MinPrice);
            }
            else if (car.MaxPrice > 0)
            {
                query = query.Where(x => x.Price <= car.MaxPrice);
            }

            if (car.Year > 0)
            {
                query = query.Where(x => x.Year.Contains(car.Year.ToString()));
            }

            if (car.Order == "Make/Model/Price")
            {
                query = query.OrderBy(x => x.MakeName)
                    .ThenBy(x => x.ModelName)
                    .ThenBy(x => x.Price)
                    .Skip((page - 1) * itemsPerPage);
            }
            else if (car.Order == "Price Asc.")
            {
                query = query.OrderBy(x => x.PriceOrder)
                    .Skip((page - 1) * itemsPerPage);
            }
            else if (car.Order == "Price Desc.")
            {
                query = query.OrderByDescending(x => x.PriceOrder)
                    .Skip((page - 1) * itemsPerPage);
            }
            else if (car.Order == "Mileage")
            {
                query = query.OrderBy(x => x.Mileage)
                    .Skip((page - 1) * itemsPerPage);
            }

            return query;
        }
    }
}
