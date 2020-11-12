namespace CarsVision.Data.Models
{
    using System.Collections.Generic;

    using CarsVision.Data.Common.Models;

    public class Make : BaseDeletableModel<int>
    {
        public Make()
        {
            this.Models = new HashSet<Model>();
        }

        public string Name { get; set; }

        public virtual ICollection<Model> Models { get; set; }
    }
}
