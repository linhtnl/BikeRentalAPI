using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Models
{
    public class Admin
    {
        public Admin()
        {
        }

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
