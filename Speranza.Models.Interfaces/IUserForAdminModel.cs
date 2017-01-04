using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
   public interface IUserForAdminModel
    {
         string Email { get; }
         string Name { get;  }
         string Surname { get; }
         string PhoneNumber { get;  }
         bool IsAdmin { get; }
         string Category { get; set; }
    }
}
