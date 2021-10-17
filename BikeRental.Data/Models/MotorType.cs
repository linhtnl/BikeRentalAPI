using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class MotorType
    {
        public MotorType()
        {
            Categories = new HashSet<Category>();
            PriceLists = new HashSet<PriceList>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<PriceList> PriceLists { get; set; }
    }
}
