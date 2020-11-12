namespace CarsVision.Data.Models
{

    using CarsVision.Data.Common.Models;

    public class Extra : BaseDeletableModel<int>
    {
        public string Type { get; set; }

        public string Name { get; set; }
    }
}
