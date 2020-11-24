namespace CarsVision.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using CarsVision.Services;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IMakesService makesService;
        private readonly ICarsService carsService;

        public HomeController(IMakesService makesService, ICarsService carsService)
        {
            this.makesService = makesService;
            this.carsService = carsService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = this.makesService.GetAllNames<MakesViewModel>();
            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(CarsSearchInputModel car)
        {
            var viewModel = this.carsService.SearchCars<CarsViewModel>(car);
            return this.View("/Views/Cars/Cars.cshtml", viewModel);
        }

        public IActionResult Ajax(string makeName)
        {
            var models = this.makesService.GetMakeModels(makeName);
            return this.Json(models);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
