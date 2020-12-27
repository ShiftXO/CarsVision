namespace CarsVision.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum Condition
    {
        Used = 0,
        New = 1,
        [Display(Name = "For parts")]
        ForParts = 2,
    }
}
