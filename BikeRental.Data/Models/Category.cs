using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Category
    {
        public Category()
        {
            Bikes = new HashSet<Bike>();
            PriceLists = new HashSet<PriceList>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public Guid? BrandId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual ICollection<Bike> Bikes { get; set; }
        public virtual ICollection<PriceList> PriceLists { get; set; }
    }
}
