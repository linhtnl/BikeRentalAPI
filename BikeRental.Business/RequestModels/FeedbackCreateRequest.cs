﻿using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class FeedbackCreateRequest
    {
        public virtual Booking IdNavigation { get; set; }
        public int rating { get; set; }
        public string content { get; set; }
    }
}