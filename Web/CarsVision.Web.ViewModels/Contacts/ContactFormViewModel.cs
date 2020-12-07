namespace CarsVision.Web.ViewModels.Contacts
{
    using System.ComponentModel.DataAnnotations;

    public class ContactFormViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your names.")]
        [Display(Name = "Your names")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter valid email.")]
        [Display(Name = "Your email")]
        public string Email { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter message title")]
        [StringLength(100, ErrorMessage = "Title must be at least {2} and no more than {1} symbols.", MinimumLength = 5)]
        [Display(Name = "Message title")]
        public string Title { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Please add message content.")]
        [StringLength(10000, ErrorMessage = "Message must be at least {2} symbols.", MinimumLength = 20)]
        [Display(Name = "Message content")]
        public string Content { get; set; }

        [GoogleReCaptchaValidation]
        public string RecaptchaValue { get; set; }
    }
}
