using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class VoucherItemCreateRequest
    {
        public Guid? CustomerId { get; set; }
        public Guid? VoucherId { get; set; }
    }
}
