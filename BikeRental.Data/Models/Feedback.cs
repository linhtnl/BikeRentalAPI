using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Feedback
    {
        public Guid Id { get; set; }
        public int? Rating { get; set; }
        public string Content { get; set; }

        public virtual Booking IdNavigation { get; set; }
    }
}
