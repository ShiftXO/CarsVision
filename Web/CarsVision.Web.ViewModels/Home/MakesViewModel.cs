namespace CarsVision.Web.ViewModels.Home
{
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class MakesViewModel : IMapFrom<Make>
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
