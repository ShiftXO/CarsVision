namespace CarsVision.Web.ViewModels.Colors
{
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class ColorsViewModel : IMapFrom<Color>
    {
        public string Name { get; set; }
    }
}
