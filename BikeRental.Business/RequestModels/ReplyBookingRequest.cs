using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class ReplyBookingRequest
    {
        public Guid OwnerId { get; set; }
        public Guid CustomerId { get; set; }
        public bool IsAccepted { get; set; }
    }
}
