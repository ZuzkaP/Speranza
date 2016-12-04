using System;
using System.Collections;
using Speranza.Database.Data.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;

namespace Speranza.Controllers
{
    public class UserManager : IUserManager
    {
        public UserCategories GetUserCategory(ICollection session)
        {
            throw new NotImplementedException();
        }

        public bool IsUserLoggedIn(ICollection session)
        {
            throw new NotImplementedException();
        }
    }
}