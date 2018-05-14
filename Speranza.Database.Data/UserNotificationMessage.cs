using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Database.Data
{
   public class UserNotificationMessage : IUserNotificationMessage
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Message { get; set; }

        public UserNotificationMessage(DateTime datefrom, DateTime dateto,string msg)
        {
            DateFrom = datefrom;
            DateTo = dateto;
            Message = msg;
        }

        public UserNotificationMessage()
        {
        }
    }
}
