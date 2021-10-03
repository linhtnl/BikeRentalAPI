using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Campaign
    {
        public Campaign()
        {
            Vouchers = new HashSet<Voucher>();
        }

        public Guid? Id { get; set; }
        public string Description { get; set; }
        public Guid? AreaId { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartingDate { get; set; }
        public int? Status { get; set; }

        public virtual Area Area { get; set; }
        public virtual ICollection<Voucher> Vouchers { get; set; }
    }
}
