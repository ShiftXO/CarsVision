namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class MakesService : IMakesService
    {
        private readonly IDeletableEntityRepository<Make> makesRepository;

        public MakesService(IDeletableEntityRepository<Make> makesRepository)
        {
            this.makesRepository = makesRepository;
        }

        public IEnumerable<T> GetAll<T>()
        {
            var query = this.makesRepository.All().OrderBy(x => x.Name);

            return query.To<T>().ToList();
        }
    }
}
