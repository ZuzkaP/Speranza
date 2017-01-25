using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
   public interface IChangePassModel
    {
         string OldPass { get; set; }
         string NewPass { get; set; }
         string ConfirmPass { get; set; }
         string Email { get; set; }
    }
}
