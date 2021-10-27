using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class ReplyBookingResponseViewModel
    {
        public Guid OwnerId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
