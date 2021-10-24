using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BikeRental.Data.ViewModels
{
    public class TokenViewModel
    {
        public TokenViewModel(Guid id, int role, string name, string phoneNumber)
        {
            Id = id;
            Role = role;
            Name = name;
            PhoneNumber = phoneNumber;
        }
        public Guid Id { get; set; }
        public int Role { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}
