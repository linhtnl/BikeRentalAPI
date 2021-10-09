using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class OwnerLoginRequest
    {
        public string AccessToken { get; set; }
        public string GoogleId { get; set; }
    }
}
