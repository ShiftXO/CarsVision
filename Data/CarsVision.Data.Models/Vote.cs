namespace CarsVision.Data.Models
{
    using CarsVision.Data.Common.Models;

    public class Vote : BaseModel<int>
    {
        public string DealershipId { get; set; }

        public virtual Dealership Dealership { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public byte Value { get; set; }
    }
}
