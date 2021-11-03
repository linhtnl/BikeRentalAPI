using BikeRental.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.RequestModels
{
    public class FeedbackCreateRequest
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
    }
}
