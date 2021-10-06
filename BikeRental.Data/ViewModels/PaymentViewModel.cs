using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class PaymentViewModel
    {
        public Guid Id { get; set; }
        public int? Method { get; set; }
        public string Name { get; set; }
        public DateTime? ActionDate { get; set; }
        public decimal? Amount { get; set; }
    }
}
