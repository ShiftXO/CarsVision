namespace CarsVision.Data.Models
{
    using System.Collections.Generic;

    using CarsVision.Data.Common.Models;

    public class Extra : BaseDeletableModel<int>
    {
        public string Name { get; set; }

        public ICollection<CarsExtras> CarsExtras { get; set; }
    }
}
