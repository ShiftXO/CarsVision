namespace CarsVision.Data.Models
{
    using System;

    using CarsVision.Data.Common.Models;

    public class Watchlist : BaseDeletableModel<string>
    {
        public Watchlist()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int CarId { get; set; }

        public virtual Car Car { get; set; }
    }
}
