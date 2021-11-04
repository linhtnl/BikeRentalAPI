using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class CustomerUpdateRequest
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Fullname { get; set; }
    }
}
