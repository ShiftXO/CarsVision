namespace CarsVision.Data.Models
{
    using System;
    using System.Collections.Generic;

    using CarsVision.Data.Common.Models;

    public class Dealership : BaseDeletableModel<string>
    {
        public Dealership()
        {
            this.Id = Guid.NewGuid().ToString();

            this.Cars = new HashSet<Car>();
            this.Votes = new HashSet<Vote>();
        }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string PhoneNumber { get; set; }

        public string LogoPictureId { get; set; }

        public virtual Picture LogoPicture { get; set; }

        public string Description { get; set; }

        public int Stars { get; set; }

        public DateTime? DealerSince { get; set; }

        public virtual ICollection<Car> Cars { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }
}
