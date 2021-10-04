using BikeRental.Business.RequestModels;
using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BikeRental.API.Models.Request
{
    public class RegisterRequest
    {
        public string AccessToken { get; set; }
        public OwnerRegisterRequest Owner { get; set; }
    }
}
