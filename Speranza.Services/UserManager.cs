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
        private IEmailManager emailManager;

        public UserManager(IDatabaseGateway db, IModelFactory factory, IDateTimeService dateTimeService, IHasher hasher, IEmailManager emailManager)
        {
            this.db = db;
            this.factory = factory;
            this.dateTimeService = dateTimeService;
            this.hasher = hasher;
            this.emailManager = emailManager;
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
            if (user != null && user.PasswordHash == passHash)
            {
                LoginResult result = new LoginResult();
                result.Category = user.Category;
                result.Email = email;
                result.IsAdmin = user.IsAdmin;

                return result;
            }
            return null;
        }

        public void RegisterNewUser(IRegisterModel model)
        {
            db.RegisterNewUser(model.Email, model.Name, model.Password, model.PhoneNumber, model.Surname);
            emailManager.SendWelcome(model.Email);
        }

        public void UpdateUserData(IUserProfileModel model)
        {
            db.UpdateUserData(model.Email, model.Name, model.Surname, model.PhoneNumber);
        }

        public IUserProfileModel GetUserProfileModelWithDataFromDB(string email)
        {
            IUser user = db.GetUserData(email);
            IUserProfileModel model = factory.CreateUserForUserProfileModel(user);

            return model;
        }

        public bool GetAllowedToSignUpFlag(string email)
        {
            return db.GetAllowedToSignUpFlag(email);
        }

        public UserCategories UpdateUserCategory(string email, UserCategories category)
        {
            int count = db.GetNumberOfVisits(email,dateTimeService.GetCurrentDate());

            if (count <= 40)
            {
                return UserCategories.Standard;
            }
            if (count <= 80)
            {
                if (category != UserCategories.Silver)
                {
                    db.SetUserCategory(email, UserCategories.Silver);
                }
                return UserCategories.Silver;
            }
            if (category != UserCategories.Gold)
            {
                db.SetUserCategory(email, UserCategories.Gold);
            }
            return UserCategories.Gold;
        }

        public void UpdateSeasonTickets()
        {
           var usersInTraining = db.GetNonProcessedUsersInTrainingBeforeDate(dateTimeService.GetCurrentDate());
            foreach (var item in usersInTraining)
            {
                IUser user = db.GetUserData(item.Email);
                db.SetAlreadyProcessedFlag(item);
                if(user.NumberOfFreeSignUpsOnSeasonTicket > 0)
                {
                    db.UpdateCountOfFreeSignUps(item.Email, -1);
                }
            }
        }
    }
}