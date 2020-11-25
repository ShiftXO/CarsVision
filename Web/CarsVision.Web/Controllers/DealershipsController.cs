namespace CarsVision.Web.Controllers
{
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Dealerships;
    using Microsoft.AspNetCore.Mvc;

    public class DealershipsController : Controller
    {
        private readonly IDealershipsService dealershipsService;

        public DealershipsController(IDealershipsService dealershipsService)
        {
            this.dealershipsService = dealershipsService;
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

        public IActionResult Id()
        {
            return this.View();
        }
    }
}
