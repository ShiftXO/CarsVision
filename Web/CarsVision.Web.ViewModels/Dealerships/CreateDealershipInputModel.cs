namespace CarsVision.Web.ViewModels.Dealerships
{

    using Microsoft.AspNetCore.Http;

    public class CreateDealershipInputModel
    {
        public string Email { get; set; }

        public string DealershipName { get; set; }

        public string Location { get; set; }

        public string FullAddress { get; set; }

        public string PhoneNumber { get; set; }

        public string Description { get; set; }

        public IFormFile LogoPicture { get; set; }
    }
}
