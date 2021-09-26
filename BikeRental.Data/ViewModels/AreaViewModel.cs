using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class AreaViewModel
    {
        public Guid Id { get; set; }
        public string PostalCode { get; set; }
        public string Name { get; set; }
    }
}
