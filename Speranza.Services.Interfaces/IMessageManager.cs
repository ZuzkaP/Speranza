using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface IMessageManager
    {
        void AddNewInfoMessage(DateTime dateTimeFrom, DateTime dateTimeTo, string message);
        IUserNotificationMessage GetMessageForCurrentDate();
    }
}
