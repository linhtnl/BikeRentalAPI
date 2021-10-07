using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Area
    {
        public Area()
        {
            Campaigns = new HashSet<Campaign>();
            Owners = new HashSet<Owner>();
            PriceLists = new HashSet<PriceList>();
        }

        public Guid Id { get; set; }
        public int? PostalCode { get; set; }
        public string Name { get; set; }
        public int? Status { get; set; }

        public virtual ICollection<Campaign> Campaigns { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
        public virtual ICollection<PriceList> PriceLists { get; set; }
    }
}
