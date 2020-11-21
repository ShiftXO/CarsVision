namespace CarsVision.Web.Controllers
{
    using CarsVision.Services;
    using Microsoft.AspNetCore.Mvc;

    public class TestController : Controller
    {
        private readonly ICarsScrapperService carsScrapperService;

        public TestController(ICarsScrapperService carsScrapperService)
        {
            this.carsScrapperService = carsScrapperService;
        }

        public IActionResult PopulateDb()
        {
            this.carsScrapperService.PopulateDb(10);
            return this.View();
        }
    }
}
