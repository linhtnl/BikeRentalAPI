using BikeRental.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class PriorityUtil
    {
        public static double GetPriority(double bookingTimes, double totalBike, double distance, double rating, double deniedTime, List<double> maxMinDistances)
        {
            double maxDistance = maxMinDistances[1];

            double priority = 0;

            double denyCoefficient = 0;
            if (deniedTime < 2)
            {
                denyCoefficient = 0.1;
            }
            else if (deniedTime > 2)
            {
                denyCoefficient = 0.15;
            }
            else if (deniedTime > 5)
            {
                denyCoefficient = 0.3;
            }

            if (deniedTime == 0)
            {
                priority = 0.5 * (1 - bookingTimes / totalBike) + 0.4 * (1 - distance / maxDistance) + 0.1 * (rating / 5);
            }
            else
            {
                priority = 0.5 * (1 - bookingTimes / totalBike) + 0.4 * (1 - distance / maxDistance) + 0.1 * (rating / 5) - (denyCoefficient * deniedTime);
            }
            return priority;
        }

        public static List<double> GetMaxMinDistance(List<OwnerByAreaViewModel> list)
        {
            double min = 0;
            double max = 0;
            foreach (var owner in list)
            {
                var distance = owner.LocationInfo.Distance;
                if (distance < min)
                    min = double.Parse(distance.ToString());
                else if (distance > max)
                    max = double.Parse(distance.ToString());
            }
            return new List<double>() { min, max };
        }
    }
}
