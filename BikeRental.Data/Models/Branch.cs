using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Branch
    {
        public Branch()
        {
            Categories = new HashSet<Category>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Category> Categories { get; set; }
    }
}
