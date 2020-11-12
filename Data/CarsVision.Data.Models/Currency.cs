namespace CarsVision.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum Currency
    {
        [Display(Name = "")]
        Unknown = 0,
        [Display(Name = "лв.")]
        BNG = 1,
        USD = 2,
        EUR = 3,
    }
}
