namespace CarsVision.Web.ViewModels.Cars
{
    using System.Collections.Generic;

    using CarsVision.Web.ViewModels.Colors;
    using CarsVision.Web.ViewModels.Home;

    public class CarPostViewModel
    {
        public IEnumerable<MakesViewModel> Makes { get; set; }

        public IEnumerable<ColorsViewModel> Colors { get; set; }
    }
}
