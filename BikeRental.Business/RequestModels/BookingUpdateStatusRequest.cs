using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class BookingUpdateStatusRequest
    {
        public Guid Id { get; set; }
        public int Status { get; set; }
    }
}
