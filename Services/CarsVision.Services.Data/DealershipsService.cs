namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class DealershipsService : IDealershipsService
    {
        private readonly IDeletableEntityRepository<Dealership> dealershipRepository;

        public DealershipsService(IDeletableEntityRepository<Dealership> dealershipRepository)
        {
            this.dealershipRepository = dealershipRepository;
        }

        public IEnumerable<T> GetAll<T>(int page, int itemsPerPage)
        {
            var dealerships = this.dealershipRepository.AllAsNoTracking()
                .OrderByDescending(x => x.Id)
                .Skip((page - 1) * itemsPerPage).Take(itemsPerPage)
                .To<T>().ToList();
            return dealerships;
        }

        public T GetById<T>(string id)
        {
            var dealership = this.dealershipRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .To<T>().FirstOrDefault();

            return dealership;
        }

        public int GetCount()
        {
            return this.dealershipRepository.AllAsNoTracking().Count();
        }
    }
}
