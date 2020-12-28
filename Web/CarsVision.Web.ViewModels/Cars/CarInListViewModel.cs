namespace CarsVision.Web.ViewModels.Cars
{
    using System;
    using System.Linq;

    using AutoMapper;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class CarInListViewModel : IMapFrom<Car>, IHaveCustomMappings
    {
        public int Id { get; set; }

        public bool IsInWatchlist { get; set; }

        public string UserPhoneNumber { get; set; }

        public string PictureUrl { get; set; }

        public string MakeName { get; set; }

        public string ModelName { get; set; }

        public int Year { get; set; }

        public int Mileage { get; set; }

        public DateTime CreatedOn { get; set; }

        public decimal Price { get; set; }

        public string Currency { get; set; }

        public string ColorName { get; set; }

        public string Location { get; set; }

        public string Modification { get; set; }

        public string CarTitle => string.Concat(this.MakeName, " ", this.ModelName, " ", this.Modification, "...");

        public string Description { get; set; }

        public decimal PriceOrder { get; set; }

        public Gearbox Gearbox { get; set; }

        public EngineType EngineType { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Car, CarInListViewModel>()
                .ForMember(x => x.PictureUrl, opt =>
                    opt.MapFrom(x =>
                        x.ImageUrl != null ?
                        x.ImageUrl :
                        "/images/cars/" + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Id + "." + x.Pictures.OrderBy(x => x.CreatedOn).FirstOrDefault().Extension))
                .ForMember(x => x.Currency, opt =>
                  opt.MapFrom(x => x.Currency))
                .ForMember(x => x.UserPhoneNumber, opt =>
                  opt.MapFrom(x => x.User.PhoneNumber));
        }
    }
}
