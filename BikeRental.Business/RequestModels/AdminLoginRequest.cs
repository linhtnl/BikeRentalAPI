using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class AdminLoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
