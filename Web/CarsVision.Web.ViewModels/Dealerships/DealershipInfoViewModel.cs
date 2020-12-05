namespace CarsVision.Web.ViewModels.Dealerships
{
    using System;
    using System.Linq;

    using AutoMapper;
    using CarsVision.Data.Models;
    using CarsVision.Services.Mapping;

    public class DealershipInfoViewModel : IMapFrom<Dealership>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string UserId { get; set; }

        public string Name { get; set; }

        public string LogoPicture { get; set; }

        public string PhoneNumber { get; set; }

        public string Location { get; set; }

        public DateTime CreatedOn { get; set; }

        public double AverageVote { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Dealership, DealershipInfoViewModel>()
                .ForMember(x => x.LogoPicture, opt =>
                    opt.MapFrom(x =>
                        "/images/dealerships/" + x.LogoPicture.Id + "." + x.LogoPicture.Extension))
                .ForMember(x => x.AverageVote, opt =>
                    opt.MapFrom(x => x.Votes.Count() == 0 ? 0 : x.Votes.Average(v => v.Value)));
        }
    }
}
