using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Payment
    {
        public Payment()
        {
            Bookings = new HashSet<Booking>();
        }

        public Guid Id { get; set; }
        public int? Method { get; set; }
        public string Name { get; set; }
        public DateTime? ActionDate { get; set; }
        public decimal? Amount { get; set; }

        public virtual ICollection<Booking> Bookings { get; set; }
    }
}
