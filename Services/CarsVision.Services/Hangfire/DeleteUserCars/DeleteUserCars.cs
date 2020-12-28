namespace CarsVision.Services.Hangfire.DeleteUserCars
{
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;

    public class DeleteUserCars : IDeleteUserCars
    {
        private readonly IDeletableEntityRepository<Car> carsRepository;

        public DeleteUserCars(IDeletableEntityRepository<Car> carsRepository)
        {
            this.carsRepository = carsRepository;
        }

        public async Task DeleteCars()
        {
            var cars = this.carsRepository.All();

            foreach (var car in cars)
            {
                if (car.Validity == 0)
                {
                    this.carsRepository.Delete(car);
                }
                else
                {
                    car.Validity--;
                }
            }

            await this.carsRepository.SaveChangesAsync();
        }
    }
}
