using System;
using System.Collections.Generic;

#nullable disable

namespace BikeRental.Data.Models
{
    public partial class Admin
    {
        public Admin()
        {
            Customers = new HashSet<Customer>();
            Owners = new HashSet<Owner>();
        }

        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<Owner> Owners { get; set; }
    }
}
