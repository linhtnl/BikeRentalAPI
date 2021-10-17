using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class PriceList
    {
        public Guid AreaId { get; set; }
        public Guid MotorTypeId { get; set; }
        public decimal? Price { get; set; }

        public virtual Area Area { get; set; }
        public virtual MotorType MotorType { get; set; }
    }
}
