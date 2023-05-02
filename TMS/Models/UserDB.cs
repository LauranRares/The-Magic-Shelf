using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TMS.Models
{
    public class UserDB : IdentityUser
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public string Address { get; set; }

        public string PhoneNr { get; set; }

        public string CreditCard { get; set; }

        public string Pet { get; set; }
    }
}
