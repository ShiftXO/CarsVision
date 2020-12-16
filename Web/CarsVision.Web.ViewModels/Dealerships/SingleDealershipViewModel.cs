namespace CarsVision.Web.ViewModels.Dealerships
{
    using System;
    using System.Collections.Generic;

    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;

    public class SingleDealershipViewModel : PagingViewModel, IMapFrom<Dealership>, IMapFrom<Car>
    {
        public IEnumerable<CarInListViewModel> DealershipCars { get; set; }

        public virtual DealershipInfoViewModel DealershipInfo { get; set; }
    }
}
