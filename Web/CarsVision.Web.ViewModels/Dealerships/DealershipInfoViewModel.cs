namespace CarsVision.Web.ViewModels.Dealerships
{
    using System;

    using AutoMapper;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class DealershipInfoViewModel : IMapFrom<Dealership>, IHaveCustomMappings
    {
        public string UserId { get; set; }

        public string Name { get; set; }

        public string LogoPicture { get; set; }

        public string PhoneNumber { get; set; }

        public string Location { get; set; }

        public DateTime CreatedOn { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Dealership, DealershipInfoViewModel>()
                .ForMember(x => x.LogoPicture, opt =>
                    opt.MapFrom(x =>
                        "/images/dealerships/" + x.LogoPicture.Id + "." + x.LogoPicture.Extension));
        }
    }
}
