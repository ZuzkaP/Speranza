using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models.Interfaces;

namespace Speranza.Models
{
    public enum AdminUsersInfoMessage
    {
        MessageInPast = 0,
        MessageSuccessfullyAdded = 1,
        MessageIsTooLong = 2,
        MessageIsEmpty = 3
    }
    public class UserNotificationMessageModel : IUserNotificationMessageModel
    {
        public string Message { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public AdminUsersInfoMessage Status { get; set; }
    }
}
