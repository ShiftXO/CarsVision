namespace CarsVision.Data.Models
{
    public class CarsExtras
    {
        public int Id { get; set; }

        public int CarId { get; set; }

        public virtual Car Car { get; set; }

        public int ExtraId { get; set; }

        public virtual Extra Extra { get; set; }
    }
}
