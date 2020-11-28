namespace CarsVision.Data.Models
{
    using System;

    using System.ComponentModel.DataAnnotations.Schema;

    using CarsVision.Data.Common.Models;

    public class Picture : BaseDeletableModel<string>
    {
        public Picture()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Extension { get; set; }

        public string RemotePictureUrl { get; set; }

        public int? CarId { get; set; }

        public virtual Car Car { get; set; }

        public string DealerId { get; set; }

        public virtual Dealership Dealership { get; set; }
    }
}
