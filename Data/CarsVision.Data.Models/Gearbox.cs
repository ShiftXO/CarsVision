namespace CarsVision.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum Gearbox
    {
        [Display(Name = "All")]
        None = 0,
        Manual = 1,
        Automatic = 2,
        [Display(Name = "Semi Automatic")]
        Semi_Automatic = 3,
    }
}
