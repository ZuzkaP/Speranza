using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
   public interface IUserNotificationMessageModel
    {
         string Message { get; set; }
         DateTime DateFrom { get; set; }
         DateTime DateTo { get; set; }
    }
}
