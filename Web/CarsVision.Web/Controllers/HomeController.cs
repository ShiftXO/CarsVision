namespace CarsVision.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Services;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Home;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly IMakesService makesService;
        private readonly ICarsService carsService;
        private readonly IUsersService usersService;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(
            IMakesService makesService,
            ICarsService carsService,
            IUsersService usersService,
            UserManager<ApplicationUser> userManager)
        {
            this.makesService = makesService;
            this.carsService = carsService;
            this.usersService = usersService;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var viewModel = this.makesService.GetAllNames<MakesViewModel>();
            return this.View(viewModel);
        }

        [HttpPost]
        public IActionResult Index(CarsSearchInputModel car, int id = 1)
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
                Cars = this.carsService.SearchCars<CarInListViewModel>(car, id, ItemsPerPage),
            };

            return this.View("/Views/Cars/All.cshtml", viewModel);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> MyCars(int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            var user = await this.userManager.GetUserAsync(this.User);

            const int ItemsPerPage = 12;
            var viewModel = new CarsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                CarsCount = this.usersService.GetCount(user.Id),
                Cars = await this.usersService.GetAll(id, ItemsPerPage, user.Id),
            };
            return this.View(viewModel);
        }

        [Authorize]
        public IActionResult Id(int id)
        {
            var car = this.carsService.GetById<SingleCarViewModel>(id);
            return this.View(car);
        }

        public IActionResult Ajax(string makeName)
        {
            var models = this.makesService.GetMakeModels(makeName);
            return this.Json(models);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Add(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var result = await this.usersService.AddCarToWatchlist(id, user.Id);

            return this.Ok(result);
        }

        [HttpGet]
        [Authorize]
        public IActionResult Edit(int id)
        {
            var viewModel = this.usersService.GetCarById(id);
            return this.View(viewModel);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(CarEditViewModel input)
        {
            if (!this.ModelState.IsValid)
            {
                var makesAndColors = this.carsService.GetAllMakesAndColors();
                var viewModel = input;
                viewModel.Makes = makesAndColors.Makes;
                viewModel.Colors = makesAndColors.Colors;
                return this.View(viewModel);
            }

            await this.carsService.Update(input);
            return this.Redirect("/Home/MyCars");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            await this.carsService.Delete(id);
            return this.Redirect("/Home/MyCars");
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
