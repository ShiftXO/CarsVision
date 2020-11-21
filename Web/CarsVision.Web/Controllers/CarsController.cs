namespace CarsVision.Web.Controllers
{
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class CarsController : Controller
    {
        private readonly IMakesService makesService;
        private readonly ICarsService carsService;
        private readonly UserManager<ApplicationUser> userManager;

        public CarsController(IMakesService makesService, ICarsService carsService, UserManager<ApplicationUser> userManager)
        {
            this.makesService = makesService;
            this.carsService = carsService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Cars()
        {
            return this.View();
        }

        [HttpGet]
        public IActionResult PostCar()
        {
            var viewModel = this.makesService.GetAllNames<MakesViewModel>();
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PostCar(CarInputModel car)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(car);
            }

            var userId = this.userManager.GetUserId(this.User);

            await this.carsService.AddCarAsync(car, userId);

            return this.Redirect("/");
        }
    }
}
