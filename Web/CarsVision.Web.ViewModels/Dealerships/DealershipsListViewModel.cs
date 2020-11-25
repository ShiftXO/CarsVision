namespace CarsVision.Web.ViewModels.Dealerships
{
    using System.Collections.Generic;

    public class DealershipsListViewModel : PagingViewModel
    {
        public IEnumerable<DealershipInListViewModel> Dealerships { get; set; }
    }
}
