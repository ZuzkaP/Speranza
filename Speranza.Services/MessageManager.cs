using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database;
using Speranza.Database.Data;
using Speranza.Database.Data.Interfaces;
using Speranza.Services.Interfaces;

namespace Speranza.Services
{
   public class MessageManager : IMessageManager
    {
        private IDatabaseGateway db;

        public MessageManager(IDatabaseGateway db)
        {
            this.db = db;
        }
        public void AddNewInfoMessage(DateTime dateFrom, DateTime dateTo, string message)
        {
            db.AddNewMessage(dateFrom, dateTo, message);
        }
        
        public IUserNotificationMessage GetMessageForCurrentDate()
        {
            IUserNotificationMessage message = new UserNotificationMessage();
            var messageFromDB = db.GetMessageForCurrentDate();
            if (messageFromDB != null)
            {
                message.Message = messageFromDB.Message ?? "";
                message.DateFrom = messageFromDB.DateFrom;
                message.DateTo = messageFromDB.DateTo;
            }
            return message; 
        }
    }
}
