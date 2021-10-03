﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class CampaignViewModel
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public Guid? AreaId { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime StartingDate { get; set; }
        public int? Status { get; set; }
    }
}
