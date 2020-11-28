namespace CarsVision.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CarsVision.Common;
    using CarsVision.Data.Common.Repositories;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;
    using CarsVision.Web.ViewModels.Dealerships;

    public class DealershipsService : IDealershipsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png" };

        private readonly IDeletableEntityRepository<Dealership> dealershipRepository;

        public DealershipsService(IDeletableEntityRepository<Dealership> dealershipRepository)
        {
            this.dealershipRepository = dealershipRepository;
        }

        public async Task<bool> CreateDealershipAsync(CreateDealershipInputModel input, ApplicationUser user, string picturePath)
        {
            var dealership = new Dealership
            {
                Name = input.DealershipName,
                DealerSince = DateTime.UtcNow,
                Location = input.Location,
                PhoneNumber = input.PhoneNumber,
                Stars = 0,
                Description = input.Description,
                User = user,
                UserId = user.Id,
            };

            // /wwwroot/images/cars/jhdsi-343g3h453-=g34g.jpg
            Directory.CreateDirectory($"{picturePath}/dealerships/");
            var extension = Path.GetExtension(input.LogoPicture.FileName).TrimStart('.');
            if (!this.allowedExtensions.Any(x => extension.EndsWith(x)))
            {
                throw new Exception($"Invalid picture extension {extension}");
            }

            var dbPicture = new Picture
            {
                Extension = extension,
            };
            dealership.LogoPicture = dbPicture;

            var physicalPath = $"{picturePath}/dealerships/{dbPicture.Id}.{extension}";
            using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
            await input.LogoPicture.CopyToAsync(fileStream);

            if (dealership != null)
            {
                await this.dealershipRepository.AddAsync(dealership);
                await this.dealershipRepository.SaveChangesAsync();

                return true;
            }

            throw new InvalidOperationException(GlobalConstants.InvalidOperationExceptionWhileCreatingDealership);
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
