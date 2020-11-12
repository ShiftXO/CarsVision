namespace CarsVision.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    public enum EuroStandard
    {
        [Display(Name = "")]
        Unknown = 0,
        [Display(Name = "Euro 1")]
        Euro_1 = 1,
        [Display(Name = "Euro 2")]
        Euro_2 = 2,
        [Display(Name = "Euro 3")]
        Euro_3 = 3,
        [Display(Name = "Euro 4")]
        Euro_4 = 4,
        [Display(Name = "Euro 5")]
        Euro_5 = 5,
        [Display(Name = "Euro 6")]
        Euro_6 = 6,
    }
}
