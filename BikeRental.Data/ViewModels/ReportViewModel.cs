using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class ReportViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    }
}
