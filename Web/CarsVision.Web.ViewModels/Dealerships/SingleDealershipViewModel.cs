namespace CarsVision.Web.ViewModels.Dealerships
{
    using System;
    using System.Collections.Generic;

    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Cars;

    public class SingleDealershipViewModel : IMapFrom<Dealership>, IMapFrom<Car>
    {
        public IEnumerable<CarInListViewModel> DealershipCars { get; set; }

        public virtual DealershipInfoViewModel DealershipInfo { get; set; }

        public int PageNumber { get; set; }

        public bool HasPreviousPage => this.PageNumber > 1;

        public int PreviousPageNumber => this.PageNumber - 1;

        public bool HasNextPage => this.PageNumber < this.PagesCount;

        public int NextPageNumber => this.PageNumber + 1;

        public int PagesCount => (int)Math.Ceiling((double)this.CarsCount / this.ItemsPerPage);

        public int CarsCount { get; set; }

        public int ItemsPerPage { get; set; }
    }
}
