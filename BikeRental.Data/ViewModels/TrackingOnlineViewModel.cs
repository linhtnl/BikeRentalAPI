using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class TrackingOnlineViewModel
    {
        public DateTime LoginTime { get; set; }
        public DateTime ExpiredTime { get; set; }

        [JsonConstructor]
        public TrackingOnlineViewModel()
        {
        }
    }
}
