namespace CarsVision.Web.Controllers
{
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Cars;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class WatchlistsController : Controller
    {
        private readonly IWatchlistsService watchlistsService;
        private readonly UserManager<ApplicationUser> userManager;

        public WatchlistsController(IWatchlistsService watchlistsService, UserManager<ApplicationUser> userManager)
        {
            this.watchlistsService = watchlistsService;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> All(int id = 1)
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
                CarsCount = this.watchlistsService.GetCount(user.Id),
                Cars = this.watchlistsService.GetAll(id, ItemsPerPage, user.Id),
            };

            return this.View(viewModel);
        }
    }
}
