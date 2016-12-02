using Speranza.Database.Data.Interfaces;
using Speranza.Models;
using System.Collections;

namespace Speranza.Services.Interfaces
{
    public interface IUserManager
    {
        bool IsUserLoggedIn(ICollection session);
        UserCategories GetUserCategory(ICollection session);
    }
}