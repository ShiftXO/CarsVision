namespace CarsVision.Web.ViewModels.Home
{
    using System.Collections.Generic;

    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class MakesViewModel : IMapFrom<Make>
    {
        public string Name { get; set; }
    }
}
