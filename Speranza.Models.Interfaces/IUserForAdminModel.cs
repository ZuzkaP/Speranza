using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
   public interface IUserForAdminModel
    {
         string Email { get; set; }
         string Name { get; set; }
         string Surname { get; set; }
         string PhoneNumber { get; set; }
    }
}
