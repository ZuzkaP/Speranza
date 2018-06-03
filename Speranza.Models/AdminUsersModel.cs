using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Web;

namespace Speranza.Models
{
    public enum AdminUsersMessages
    {
        NoMessage = 0,
        SuccessfullyClearAdminRole = 1,
        SuccessfullySetAdminRole = 2,
        SuccessfullyChangedCategory = 3,
        SuccessfullyIncreasedCountOfSignUps = 4,
        SuccessfullyDecreasedCountOfSignUps = 5,
        SuccessfullyUserSignOffFromTraining = 6
    }

    public class AdminUsersModel 
    {
        public IList<string> Categories { get;private set; }
        public IList<IUserForAdminModel> Users { get; set; }

        public IUserNotificationMessageModel MessageModel { get; set; }

        public AdminUsersModel()
        {
            Categories = new List<string>();
            MessageModel = new UserNotificationMessageModel();
        }
    }
}
