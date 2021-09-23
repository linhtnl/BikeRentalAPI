using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class PriceList
    {
        public string CategoryId { get; set; }
        public decimal? Price { get; set; }
        public string AreaId { get; set; }

        public virtual Area Area { get; set; }
        public virtual Category Category { get; set; }
    }
}
