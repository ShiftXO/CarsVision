namespace CarsVision.Web.ViewModels.Cars
{
    using System.Collections.Generic;

    public class CarsListViewModel : PagingViewModel
    {
        public IEnumerable<CarInListViewModel> Cars { get; set; }
    }
}
