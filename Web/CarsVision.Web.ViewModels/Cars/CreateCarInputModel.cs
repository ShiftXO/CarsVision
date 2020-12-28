namespace CarsVision.Web.ViewModels.Cars
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using CarsVision.Data.Models;
    using Microsoft.AspNetCore.Http;

    public class CreateCarInputModel
    {
        [Required]
        public string Make { get; set; }

        [Required]
        public string Model { get; set; }

        public string Modification { get; set; }

        [Required]
        [Range(1, 7)]
        public EngineType EngineType { get; set; }

        public int Power { get; set; }

        [Required]
        public IEnumerable<IFormFile> Pictures { get; set; }

        public EuroStandard EuroStandard { get; set; }

        [Required]
        [Range(1, 3)]
        public Gearbox Gearbox { get; set; }

        [Required]
        [Range(1, 11)]
        public Category Category { get; set; }

        [Required]
        [Range(0, 2_000_000)]
        public int Price { get; set; }

        [Required]
        [Range(1, 3)]
        public Currency Currency { get; set; }

        [Required]
        [Range(1, 12)]
        public byte Month { get; set; }

        // [CurrentYearMaxValue(1930)]
        [Required]
        public int Year { get; set; }

        [Required]
        public int Mileage { get; set; }

        public string Color { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        public byte Validity { get; set; }

        public bool IsVIP { get; set; }

        public string Description { get; set; }

        public Condition Condition { get; set; }

        public IEnumerable<int> Extras { get; set; }
    }
}
