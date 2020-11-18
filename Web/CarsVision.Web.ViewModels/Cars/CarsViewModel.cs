namespace CarsVision.Web.ViewModels.Cars
{
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class CarsViewModel : IMapFrom<Car>
    {
        public string Id { get; set; }

        public string MakeName { get; set; }

        public string ModelName { get; set; }

        public string Modification { get; set; }

        public string Year { get; set; }

        public bool IsVIP { get; set; }

        public bool IsFromDealer { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string Location { get; set; }

        public string Color { get; set; }

        public string Mileage { get; set; }
    }
}
