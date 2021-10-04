using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Models.Request
{
    public class LoginRequest
    {
        public string AccessToken { get; set; }
        public string GoogleId { get; set; }
    }
}
