using Speranza.Common.Data;
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
        int UpdateCountOfFreeSignUps(string email, int changeNumberOfSignUps);
        IList<ITrainingModel> GetFutureTrainingsForUser(string v);
        void ChangePassword(string email, string newPass);
        IList<IUserForTrainingDetailModel> GetAllUsersForTrainingDetails();
        void UpdateSeasonTickets();
        bool UserExists(string email);
        IUserForTrainingDetailModel GetAddedUserData(string email);
        ILoginResult Login(string email, string passHash);
        void RegisterNewUser(IRegisterModel model);
        void PromptToConfirmUserAttendance();
        void UpdateUserData(IUserProfileModel model);
        IUserProfileModel GetUserProfileModelWithDataFromDB(string email);
        bool GetAllowedToSignUpFlag(string email);
        UserCategories UpdateUserCategory(string email, UserCategories category);
    }
}