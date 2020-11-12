namespace CarsVision.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum Category
    {
        [Display(Name = "")]
        Unknown = 0,
        Van = 1,
        SUV = 2,
        Convertible = 3,
        Wagon = 4,
        Coupe = 5,
        Minivan = 6,
        Pickup = 7,
        Sedan = 8,
        [Display(Name = "Stretch Limousine")]
        Stretch_Limousine = 9,
        Limousine = 10,
        Hatchback = 11,
    }
}
