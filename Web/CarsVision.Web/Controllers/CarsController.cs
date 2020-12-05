namespace CarsVision.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Cars;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class CarsController : BaseController
    {
        private readonly ICarsService carsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;

        public CarsController(
            ICarsService carsService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment)
        {
            this.carsService = carsService;
            this.userManager = userManager;
            this.environment = environment;
        }

        [HttpGet]
        public async Task<IActionResult> All(string order, int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var userId = user != null ? user.Id : string.Empty;

            const int ItemsPerPage = 12;
            var viewModel = new CarsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                CarsCount = this.carsService.GetCount(),
                Cars = await this.carsService.GetAll(id, userId, ItemsPerPage),
            };

            return this.View(viewModel);
        }

        [HttpGet]
        [Authorize]
        public IActionResult PostCar()
        {
            var viewModel = this.carsService.GetAllMakesAndColors();
            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostCar(CreateCarInputModel input)
        {
            if (!this.ModelState.IsValid)
            {
                var viewModel = this.carsService.GetAllMakesAndColors();
                return this.View(viewModel);
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

        public IActionResult Id(int id)
        {
            var car = this.carsService.GetById(id);
            return this.View(car);
        }
    }
}
