using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Web;

namespace Speranza.Models
{
    public enum UsersAdminMessages
    {
        NoMessage = 0,
        SuccessfullyClearAdminRole = 1,
        SuccessfullySetAdminRole = 2,
        SuccessfullyChangedCategory = 3,
        SuccessfullyIncreasedCountOfSignUps = 4,
        SuccessfullyDecreasedCountOfSignUps = 5
    }
    public class AdminUsersModel 
    {
        public IList<string> Categories { get;private set; }
        public IList<IUserForAdminModel> Users { get; set; }

        public AdminUsersModel()
        {
            Categories = new List<string>();
        }
    }
}
