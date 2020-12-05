namespace CarsVision.Web.Controllers
{
    using System.Threading.Tasks;

    using CarsVision.Data.Models;
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Dealerships;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class DealershipsController : Controller
    {
        private readonly IDealershipsService dealershipsService;
        private readonly UserManager<ApplicationUser> userManager;

        public DealershipsController(IDealershipsService dealershipsService, UserManager<ApplicationUser> userManager)
        {
            this.dealershipsService = dealershipsService;
            this.userManager = userManager;
        }

        public IActionResult All(int id = 1)
        {
            if (id <= 0)
            {
                return this.NotFound();
            }

            const int ItemsPerPage = 12;
            var viewModel = new DealershipsListViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = id,
                CarsCount = this.dealershipsService.GetCount(),
                Dealerships = this.dealershipsService.GetAll<DealershipInListViewModel>(id, ItemsPerPage),
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Id(string id, int page = 1)
        {
            if (page == 0)
            {
                page = 1;
            }

            var user = await this.userManager.GetUserAsync(this.User);

            var userId = user != null ? user.Id : string.Empty;

            const int ItemsPerPage = 12;
            var viewModel = new SingleDealershipViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = page,
                CarsCount = this.dealershipsService.GetDealershipsCarsCount(id),
                DealershipInfo = this.dealershipsService.GetDealershipInfo(id),
                DealershipCars = await this.dealershipsService.GetAllDealershipCars(page, id, userId, ItemsPerPage),
            };

            return this.View(viewModel);
        }
    }
}
