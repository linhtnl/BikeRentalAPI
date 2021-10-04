using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Owner
    {
        public Owner()
        {
            Bikes = new HashSet<Bike>();
            Bookings = new HashSet<Booking>();
        }

        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string IdentityImg { get; set; }
        public int? Status { get; set; }
        public int? BanTimes { get; set; }
        public Guid? AdminId { get; set; }
        public Guid? AreaId { get; set; }
        public string Mail { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual Area Area { get; set; }
        public virtual Wallet Wallet { get; set; }
        public virtual ICollection<Bike> Bikes { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
