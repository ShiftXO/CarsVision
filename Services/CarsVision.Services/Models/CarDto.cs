namespace CarsVision.Services.Models
{
    using System.Collections.Generic;

    using CarsVision.Data.Models;

    public class CarDto
    {
        public CarDto()
        {
            this.ExtraNames = new HashSet<string>();
        }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Modification { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; }

        public string EngineType { get; set; }

        public string Gearbox { get; set; }

        public int Power { get; set; }

        public string Year { get; set; }

        public int Mileage { get; set; }

        public string ColorName { get; set; }

        public string EuroStandard { get; set; }

        public string Currency { get; set; }

        public string Location { get; set; }

        public string Description { get; set; }

        public ICollection<string> ExtraNames { get; set; }
    }
}
