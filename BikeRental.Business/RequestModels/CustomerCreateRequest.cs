using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class CustomerCreateRequest
    {
        public string PhoneNumber { get; set; }
        public string IdentityNumber { get; set; }
        public string Fullname { get; set; }
        public string IdentityImg { get; set; }
    }
}
