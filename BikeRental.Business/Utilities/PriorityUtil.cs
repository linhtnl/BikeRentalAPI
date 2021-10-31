using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class PriorityUtil
    {
        public static double GetPriority (double bookingTimes, double totalBike, double distance, double rating, double deniedTime)
        {
            if(rating == 0)
            {
                rating = 1;
            }
            if(distance == 0)
            {
                distance = 1;
            }
            if(bookingTimes == 0)
            {
                bookingTimes = 1;
            }
            if(deniedTime == 0)
            {
                deniedTime = 1;
            }
            var priority = (bookingTimes / totalBike) * (1000 / distance) * rating * (1 / bookingTimes)*(1/deniedTime);
            return priority;
        }
    }
}
