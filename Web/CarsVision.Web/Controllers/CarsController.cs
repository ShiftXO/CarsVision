namespace CarsVision.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CarsVision.Common;
    using CarsVision.Data.Models;
    using CarsVision.Services.Data;
    using CarsVision.Services.Messaging;
    using CarsVision.Web.ViewModels.Cars;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    public class CarsController : BaseController
    {
        private readonly ICarsService carsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment environment;
        private readonly IEmailSender emailSender;
        private readonly IConfiguration configuration;

        public CarsController(
            ICarsService carsService,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment environment,
            IEmailSender emailSender,
            IConfiguration configuration)
        {
            this.carsService = carsService;
            this.userManager = userManager;
            this.environment = environment;
            this.emailSender = emailSender;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> All(string order = "Make/Model/Price", int id = 1)
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
                Order = order,
                CarsCount = this.carsService.GetCount(),
                Cars = this.carsService.GetAll(id, userId, order, ItemsPerPage),
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

            return this.Redirect("/Home/MyCars");
        }

        public IActionResult Id(int id)
        {
            this.ViewData["MapApiKey"] = this.configuration["MapToken:ApiKey"];
            var car = this.carsService.GetById(id);
            return this.View(car);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SendToEmail(int id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            var car = this.carsService.GetById(id);

            var html = new StringBuilder();
            html.AppendLine($"<h1>{car.MakeName} {car.ModelName}</h1>");
            html.AppendLine($"<h3>{car.Modification}</h3>");
            html.AppendLine($"<img src=\"{"/images/cars/" + car.PictureUrls.FirstOrDefault()}\">");
            await this.emailSender.SendEmailAsync(GlobalConstants.SendGridSender, "CarsVision", user.Email, car.MakeName, html.ToString());

            return this.RedirectToAction(nameof(this.Id), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> IncreaseViews(int id)
        {
            var views = await this.carsService.IncreaseViews(id);
            return this.Ok(views);
        }
    }
}
