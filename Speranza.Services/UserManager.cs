using System;
using System.Collections;
using Speranza.Database.Data.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;
using System.Collections.Generic;
using System.Web.SessionState;
using System.Web;
using Speranza.Models.Interfaces;
using Speranza.Database;
using System.Linq;
using Speranza.Common.Data;
using Speranza.Models;

namespace Speranza.Services
{
    public class UserManager : IUserManager
    {
        private IDatabaseGateway db;
        private IModelFactory factory;
        private IDateTimeService dateTimeService;
        private IHasher hasher;

        public UserManager(IDatabaseGateway db, IModelFactory factory, IDateTimeService dateTimeService, IHasher hasher)
        {
            this.db = db;
            this.factory = factory;
            this.dateTimeService = dateTimeService;
            this.hasher = hasher;
        }

        public UserCategories GetUserCategory(ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["Category"] != null)
                    return (UserCategories)sessionData["Category"];
            }

            return UserCategories.Standard;
        }

        public bool IsUserLoggedIn(ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["Email"] != null && (string)sessionData["Email"] != "")
                    return true;
            }

            return false;
        }

        public bool IsUserAdmin(ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["IsAdmin"] != null && (bool)sessionData["IsAdmin"] == true)
                    return true;
            }

            return false;
        }

        public IList<IUserForAdminModel> GetAllUsersForAdmin()
        {
            var usersFromDB = db.GetAllUsers();
            var modelsList = new List<IUserForAdminModel>();
            foreach (var item in usersFromDB)
            {
                IUserForAdminModel userModel = factory.CreateUserForAdminModel(item);
                modelsList.Add(userModel);
            }
            return modelsList;
        }

        public void SetUserRoleToAdmin(string email, bool isAdmin)
        {
            db.SetAdminRole(email, isAdmin);
        }

        public void SetUserCategory(string email, UserCategories category)
        {
            db.SetUserCategory(email, category);
        }

        public int UpdateCountOfFreeSignUps(string email, int changeNumberOfSignUps)
        {
            return db.UpdateCountOfFreeSignUps(email, changeNumberOfSignUps);
        }

        public IList<ITrainingModel> GetFutureTrainingsForUser(string email)
        {
            IList<ITraining> alluserTrainings = db.GetTrainingsForUser(email);
            IList<ITraining> futureUserTrainings = alluserTrainings.Where(r => r.Time > dateTimeService.GetCurrentDate()).ToList();
            IList<ITrainingModel> trainingsModels = new List<ITrainingModel>();
            foreach (var training in futureUserTrainings)
            {
                ITrainingModel model = factory.CreateTrainingModel(training);
                trainingsModels.Add(model);
            }
            return trainingsModels;
        }

        public void ChangePassword(string email, string newPass)
        {
            db.ChangePassword(email, newPass);
        }

        public IList<IUserForTrainingDetailModel> GetAllUsersForTrainingDetails()
        {
            var usersFromDB = db.GetAllUsers();
            var usersModels = new List<IUserForTrainingDetailModel>();
            foreach (var item in usersFromDB)
            {
                var model = factory.CreateUsersForTrainingDetailModel(item);
                usersModels.Add(model);
            }
            return usersModels.OrderBy(r => r.Surname).ToList();
        }

        public bool UserExists(string email)
        {
            return (db.GetUserData(email) != null);
        }

        public IUserForTrainingDetailModel GetAddedUserData(string email)
        {
            IUser user = db.GetUserData(email);
            IUserForTrainingDetailModel model = factory.CreateUsersForTrainingDetailModel(user);
            return model;
        }

        public ILoginResult Login(string email, string passHash)
        {
            IUser user = db.LoadUser(email);
            if(user != null && user.PasswordHash == passHash)
            {
                LoginResult result = new LoginResult();
                result.Category = user.Category;
                result.Email = email;
                result.IsAdmin = user.IsAdmin;
                
                return result;
            }
            return null;
        }
    }
}