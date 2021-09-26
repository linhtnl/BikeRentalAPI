using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class WalletViewModel
    {
        public Guid Id { get; set; }
        public decimal? Balance { get; set; }
        public string MomoId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
    }
}
