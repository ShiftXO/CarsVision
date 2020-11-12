namespace CarsVision.Data.Models
{
    using System;
    using System.Collections.Generic;

    using CarsVision.Data.Common.Models;

    public class Watchlist : BaseDeletableModel<string>
    {
        public Watchlist()
        {
            this.Id = Guid.NewGuid().ToString();

            this.Cars = new HashSet<Car>();
        }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public ICollection<Car> Cars { get; set; }
    }
}
