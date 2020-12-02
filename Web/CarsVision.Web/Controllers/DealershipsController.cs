namespace CarsVision.Web.Controllers
{
    using CarsVision.Services.Data;
    using CarsVision.Web.ViewModels.Cars;
    using CarsVision.Web.ViewModels.Dealerships;
    using Microsoft.AspNetCore.Mvc;

    public class DealershipsController : Controller
    {
        private readonly IDealershipsService dealershipsService;
        private readonly ICarsService carsService;

        public DealershipsController(IDealershipsService dealershipsService, ICarsService carsService)
        {
            this.dealershipsService = dealershipsService;
            this.carsService = carsService;
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

        public IActionResult Id(string id, int page = 1)
        {
            if (page == 0)
            {
                page = 1;
            }

            const int ItemsPerPage = 12;
            var viewModel = new SingleDealershipViewModel
            {
                ItemsPerPage = ItemsPerPage,
                PageNumber = page,
                CarsCount = this.dealershipsService.GetDealershipsCarsCount(id),
                DealershipInfo = this.dealershipsService.GetDealershipInfo(id),
                DealershipCars = this.dealershipsService.GetAllDealershipCars(page, id, ItemsPerPage),
            };

            return this.View(viewModel);
        }
    }
}
