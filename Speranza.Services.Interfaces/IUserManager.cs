using Speranza.Database.Data.Interfaces;
using Speranza.Models;
using Speranza.Models.Interfaces;
using System.Collections;
using System.Collections.Generic;

namespace Speranza.Services.Interfaces
{
    public interface IUserManager
    {
        bool IsUserLoggedIn(ICollection session);
        UserCategories GetUserCategory(ICollection session);
        bool IsUserAdmin(ICollection session);
        IList<IUserForAdminModel> GetAllUsersForAdmin();
        void SetUserRoleToAdmin(string email, bool isAdmin);
        void SetUserCategory(string email, UserCategories category);
    }
}