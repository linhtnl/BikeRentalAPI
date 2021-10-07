using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class OwnerSearchRequest
    {
        public string OwnerName { get; set; }
        public int FilterOption { get; set; }
    }
}
