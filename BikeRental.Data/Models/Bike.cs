using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Bike
    {
        public Bike()
        {
            Bookings = new HashSet<Booking>();
        }

        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public string ModelYear { get; set; }
        public Guid? OwnerId { get; set; }
        public Guid? CategoryId { get; set; }
        public int? Status { get; set; }
        public string ImgPath { get; set; }

        public virtual Category Category { get; set; }
        public virtual Owner Owner { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
