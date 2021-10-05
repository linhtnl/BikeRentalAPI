using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class PricelistCreateRequest
    {    
        public Guid CategoryId { get; set; }
        public decimal? Price { get; set; }
        public Guid AreaId { get; set; }
    }
}
