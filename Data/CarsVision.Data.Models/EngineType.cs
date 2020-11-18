namespace CarsVision.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum EngineType
    {
        [Display(Name = "All")]
        Unknown = 0,
        Gasoline = 1,
        Diesel = 2,
        [Display(Name = "Natural Gas")]
        Natural_Gas = 3,
        [Display(Name = "Gasoline Мethane")]
        Gasoline_Мethane = 4,
        [Display(Name = "Hybrid (gasoline-electric)")]
        Hybrid = 5,
        Electric = 6,
        Other = 7,
    }
}
