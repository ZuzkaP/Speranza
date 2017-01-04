using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class UserForAdminModel : IUserForAdminModel
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string PhoneNumber { get; set; }

        public string Surname { get; set; }

        public bool IsAdmin { get; set; }
    }
}
