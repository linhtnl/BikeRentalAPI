using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class PriorityUtil
    {
        public static double GetPriority (int bike, int totalBike, int distance, double rating, int deniedTime)
        {
            var priority = (bike / totalBike) * (1 / distance) * rating * (1 / deniedTime);
            return priority;
        }
    }
}
