namespace CarsVision.Services.Hangfire.DeleteUserCars
{
    using System.Threading.Tasks;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;

    public class DeleteUserCars : IDeleteUserCars
    {
        private readonly IDeletableEntityRepository<ApplicationUser> usersRepository;

        public DeleteUserCars(IDeletableEntityRepository<ApplicationUser> usersRepository)
        {
            this.usersRepository = usersRepository;
        }

        public async Task DeleteCars()
        {
            // TODO: Delete cars after their validity expires.
            //var cars = this.usersRepository.All().Select(x => x.Cars).FirstOrDefault();

            //foreach (var user in users)
            //{
            //    var cars = user.Cars;
            //}

            //var car = cars.FirstOrDefault();
            //car.Price = 999;
            //await this.usersRepository.SaveChangesAsync();
        }
    }
}
