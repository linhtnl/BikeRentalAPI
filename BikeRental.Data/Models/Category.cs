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
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? BrandId { get; set; }
        public int? Status { get; set; }
        public Guid? MotorTypeId { get; set; }

        public virtual Brand Brand { get; set; }
        public virtual MotorType MotorType { get; set; }
        public virtual ICollection<Bike> Bikes { get; set; }
    }
}
