namespace CarsVision.Web.ViewModels.Cars
{
    using CarsVision.Data.Models;

    public class CarPredictInputModel
    {
        public string Make { get; set; }

        public string Model { get; set; }

        public int Power { get; set; }

        public EuroStandard EuroStandard { get; set; }

        public Gearbox Gearbox { get; set; }

        public int Price { get; set; }

        public Currency Currency { get; set; }

        public string Year { get; set; }

        public int Mileage { get; set; }
    }
}
