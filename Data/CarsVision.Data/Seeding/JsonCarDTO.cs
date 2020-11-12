namespace CarsVision.Data.Seeding
{
    using System.Collections.Generic;

    public class JsonCarDTO
    {
        public string Brand { get; set; }

        public ICollection<string> Models { get; set; }
    }
}
