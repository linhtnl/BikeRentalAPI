using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class OwnerUpdateRequest
    {
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
    }
}
