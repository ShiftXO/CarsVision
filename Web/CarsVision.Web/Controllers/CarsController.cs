namespace CarsVision.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class CarsController : Controller
    {
        private readonly IMakesService makesService;
        private readonly ICarsService carsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        public CarsController(IMakesService makesService, ICarsService carsService, UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
        {
            this.makesService = makesService;
            this.carsService = carsService;
            this.userManager = userManager;
            this.environment = environment;
        }

        [HttpGet]
        public IActionResult All(int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            const int ItemsPerPage = 12;
            var viewModel = new CarsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                CarsCount = this.carsService.GetCount(),
                Cars = this.carsService.GetAll<CarInListViewModel>(id, ItemsPerPage),
            };
            return this.View(viewModel);
        }

        [HttpGet]
        public IActionResult PostCar()
        {
            var viewModel = this.makesService.GetAllNames<MakesViewModel>();
            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PostCar(CreateCarInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            var user = await this.userManager.GetUserAsync(this.User);

            try
            {
                await this.carsService.AddCarAsync(input, user.Id, $"{this.environment.WebRootPath}/images");
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.View(input);
            }

            return this.Redirect("/");
        }
    }
}
