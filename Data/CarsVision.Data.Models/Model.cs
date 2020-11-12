namespace CarsVision.Data.Models
{
    using CarsVision.Data.Common.Models;

    public class Model : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public int MakeId { get; set; }

        public Make Make { get; set; }
    }
}
