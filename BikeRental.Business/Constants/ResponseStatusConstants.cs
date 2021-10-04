using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Business.Constants
{
    public class ResponseStatusConstants
    {
        public static readonly int CREATED = 201;

        public static readonly int BAD_REQUEST = 400;
        public static readonly int UNAUTHORIZED = 401;
        public static readonly int PAYMENT_REQUIRED = 402;
        public static readonly int FORBIDDEN = 403;
        public static readonly int NOT_FOUND = 404;
        public static readonly int METHOD_NOT_ALLOWED = 405;
    }
}
