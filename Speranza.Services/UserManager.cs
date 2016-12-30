using System;
using System.Collections;
using Speranza.Database.Data.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Web;
using Speranza.Models.Interfaces;

namespace Speranza.Services
{
    public class UserManager : IUserManager
    {
        public UserCategories GetUserCategory(ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["Category"] != null)
                    return (UserCategories) sessionData["Category"];
            }

            return UserCategories.Standard;
        }

        public bool IsUserLoggedIn(ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if(sessionData["Email"] != null && (string) sessionData["Email"] != "")
                return true;
            }
                
            return false;
        }

        public bool IsUserAdmin(ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["IsAdmin"] != null &&  (bool)sessionData["IsAdmin"] == true )
                    return true;
            }

            return false;
        }

        public IList<IUserForAdminModel> GetAllUsersForAdmin()
        {
            throw new NotImplementedException();
        }
    }
}