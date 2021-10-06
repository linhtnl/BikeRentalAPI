using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class WalletCreateRequest
    {
        public Guid Id { get; set; }
        public string MomoId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
    }
}
