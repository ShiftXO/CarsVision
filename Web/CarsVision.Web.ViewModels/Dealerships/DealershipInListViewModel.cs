namespace CarsVision.Web.ViewModels.Dealerships
{
    using System;

    using AutoMapper;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class DealershipInListViewModel : IMapFrom<Dealership>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string LogoPictureUrl { get; set; }

        public string Description { get; set; }

        public int Stars { get; set; }

        public DateTime? DealerSince { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Dealership, DealershipInListViewModel>()
                .ForMember(x => x.LogoPictureUrl, opt =>
                    opt.MapFrom(x =>
                        "/images/cars/" + x.LogoPicture.Id + "." + x.LogoPicture.Extension));
        }
    }
}
