﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class OwnerRegisterRequest
    {
        public string AccessToken { get; set; }
        public OwnerCreateRequest Owner { get; set; }
    }
}
