namespace CarsVision.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using CarsVision.Web.ViewModels.Cars;

    public class HomeViewModel
    {
        public IEnumerable<MakesViewModel> Data { get; set; }

        public IEnumerable<CarInListViewModel> Cars { get; set; }
    }
}
