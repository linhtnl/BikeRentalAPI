using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class PaymentCreateRequest
    {
        public int? Method { get; set; }
        public string Name { get; set; }
        public DateTime? ActionDate { get; set; }
        public decimal? Amount { get; set; }
    }
}
