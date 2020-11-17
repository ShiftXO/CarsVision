namespace CarsVision.Web.ViewModels.Cars
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class CarInputModel
    {
        public string Make { get; set; }

        public string Model { get; set; }

        public string Modification { get; set; }

        public int Fuel { get; set; }

        public int Power { get; set; }

        public string ImageUrl { get; set; }

        public int EuroStandart { get; set; }

        [Range(1, 3)]
        public int Gearbox { get; set; }

        [Range(1, 11)]
        public int Category { get; set; }

        public int Price { get; set; }

        public string Currency { get; set; }

        public string Month { get; set; }

        // [CurrentYearMaxValue(1930)]
        public int Year { get; set; }

        public int Mileage { get; set; }

        public int Color { get; set; }

        public string Location { get; set; }

        public string Validity { get; set; }

        public bool IsVIP { get; set; }

        public string Description { get; set; }
    }
}
