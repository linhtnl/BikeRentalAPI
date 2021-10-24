using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Utilities
{
    public class DiscountUtil
    {
        public static decimal? DiscountBooking(decimal originalPrice, int discountPercent, decimal maxDiscountAmount)
        {
            decimal discountPercentDecimal = discountPercent;

            var discountAmount = Decimal.Multiply(originalPrice, (Decimal.Divide(discountPercentDecimal, 100)));

            if (discountAmount >= maxDiscountAmount)
            {
                return originalPrice - maxDiscountAmount;
            }
            else
            {
                return originalPrice - discountAmount;
            }
        }
    }
}
