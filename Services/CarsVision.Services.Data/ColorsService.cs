namespace CarsVision.Services.Data
{
    using System.Collections.Generic;
    using System.Linq;

    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class ColorsService : IColorsService
    {
        private readonly IRepository<Color> colorRepository;

        public ColorsService(IRepository<Color> colorRepository)
        {
            this.colorRepository = colorRepository;
        }

        public IEnumerable<T> GetAll<T>()
        {
            var query = this.colorRepository.All();

            return query.To<T>().ToList();
        }
    }
}
