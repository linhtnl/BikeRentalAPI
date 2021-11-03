using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class UserLoginResponseViewModel
    {
        public string Token { get; set; }
        public string FullName { get; set; }

        public UserLoginResponseViewModel(string token, string fullName)
        {
            Token = token;
            FullName = fullName;
        }
    }
}
