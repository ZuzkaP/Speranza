using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Speranza.Models
{
    public class RegisterModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}