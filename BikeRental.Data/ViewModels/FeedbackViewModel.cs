using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class FeedbackViewModel
    {
        public Guid Id { get; set; }
        public int? Rating { get; set; }
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public int Status { get; set; }
    }
}
