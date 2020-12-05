namespace CarsVision.Web.ViewModels.Cars
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class SingleCarViewModel : IMapFrom<Car>, IMapFrom<ApplicationUser>, IMapFrom<Dealership>, IHaveCustomMappings
    {
        public string UserId { get; set; }

        public bool IsDealership { get; set; }

        public string DealershipName { get; set; }

        public int Id { get; set; }

        public string PhoneNumber { get; set; }

        public string MakeName { get; set; }

        public DateTime CreatedOn { get; set; }

        public string ModelName { get; set; }

        public string Modification { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public EngineType EngineType { get; set; }

        public Gearbox Gearbox { get; set; }

        public Category Category { get; set; }

        public int Power { get; set; }

        public string Year { get; set; }

        public int Mileage { get; set; }

        public int Views { get; set; }

        public bool IsVIP { get; set; }

        public string ColorName { get; set; }

        public EuroStandard EuroStandard { get; set; }

        public string Euro => this.EuroStandard.ToString().Replace("_", " ");

        public Currency Currency { get; set; }

        public string Location { get; set; }

        public string PictureUrl { get; set; }

        public ICollection<string> PictureUrls { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Car, SingleCarViewModel>()
                .ForMember(x => x.PictureUrl, opt =>
                    opt.MapFrom(x =>
                        x.ImageUrl != null ?
                        x.ImageUrl :
                        "/images/cars/" + x.Pictures.FirstOrDefault().Id + "." + x.Pictures.FirstOrDefault().Extension));
        }
    }
}
