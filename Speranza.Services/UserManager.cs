using System;
using System.Collections;
using Speranza.Database.Data.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;
using System.Collections.Generic;
using System.Web.SessionState;

namespace Speranza.Services
{
    public class UserManager : IUserManager
    {
        public UserCategories GetUserCategory(ICollection session)
        {
            SessionStateItemCollection sessionData = session as SessionStateItemCollection;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["Category"] != null)
                    return (UserCategories) sessionData["Category"];
            }

            return UserCategories.Standard;
        }

        public bool IsUserLoggedIn(ICollection session)
        {
            
            SessionStateItemCollection sessionData = session as SessionStateItemCollection;
            if (sessionData != null && sessionData.Count != 0)
            {
                if(sessionData["Email"] != null && (string) sessionData["Email"] != "")
                return true;
            }
                
            return false;
        }
    }
}