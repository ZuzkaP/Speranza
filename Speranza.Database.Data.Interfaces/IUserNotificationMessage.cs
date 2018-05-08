using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data.Interfaces
{
    public interface IUserNotificationMessage
    {
        DateTime DateFrom { get; set; }
        DateTime DateTo { get; set; }
        string Message { get; set; }
    }
}
