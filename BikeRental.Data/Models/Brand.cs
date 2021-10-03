using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Brand
    {
        public Brand()
        {
            Categories = new HashSet<Category>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
