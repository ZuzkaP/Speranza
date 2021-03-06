﻿using Speranza.Common.Data;
using Speranza.Database.Data.Interfaces;
using Speranza.Models;
using Speranza.Models.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Speranza.Services.Interfaces
{
    public interface IUserManager
    {
        bool IsUserLoggedIn(string cookie,ICollection session);
        UserCategories GetUserCategory(ICollection session);
        bool IsUserAdmin(ICollection session);
        IList<IUserForAdminModel> GetAllUsersForAdmin();
        void SetUserRoleToAdmin(string email, bool isAdmin);
        void SetUserCategory(string email, UserCategories category);
        int UpdateCountOfFreeSignUps(string email, int changeNumberOfSignUps);
        IList<ITrainingModel> GetFutureTrainingsForUser(string v);
        bool SendNewPass(string email);
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
        void CleanUpTokens();
        UserCategories UpdateUserCategory(string email, UserCategories category);
        void SetRememberMe(string email, string cookieSeries, string cookieToken);
        ILoginResult VerifyRememberMe(string cookie);
        void CancelRememberMe(string email);

        void RemoveAccountFromDB(string email);
        bool ToggleAllowUserToSignUp(string email);
    }
}