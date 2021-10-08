using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class BikeDeleteSuccessViewModel
    {
        public Guid Id { get; set; }
        public string LicensePlate { get; set; }
        public string Color { get; set; }
        public string ModelYear { get; set; }
        public int? Status { get; set; }
    }
}
