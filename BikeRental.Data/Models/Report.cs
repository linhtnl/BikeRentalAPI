using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Report
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        public virtual Booking IdNavigation { get; set; }
    }
}
