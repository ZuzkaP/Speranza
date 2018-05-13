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
using Speranza.Database.Data;
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
        private IUidService uidService;

        public UserManager(IDatabaseGateway db, IModelFactory factory, IDateTimeService dateTimeService, IHasher hasher, IEmailManager emailManager, IUidService uidService)
        {
            this.db = db;
            this.factory = factory;
            this.dateTimeService = dateTimeService;
            this.hasher = hasher;
            this.emailManager = emailManager;
            this.uidService = uidService;
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

        public bool IsUserLoggedIn(string cookie, ICollection session)
        {
            HttpSessionStateBase sessionData = session as HttpSessionStateBase;
            if (sessionData != null && sessionData.Count != 0)
            {
                if (sessionData["Email"] != null && (string)sessionData["Email"] != "")
                    return true;
            }
            var user = VerifyRememberMe(cookie);
            if(user != null)
            {
                sessionData["Email"] = user.Email;
                sessionData["Category"] = user.Category;
                sessionData["IsAdmin"] = user.IsAdmin;
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
            IList<ITraining> futureUserTrainings = alluserTrainings.Where(r => r.Time > dateTimeService.GetCurrentDateTime()).ToList();
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
            int count = db.GetNumberOfVisits(email, dateTimeService.GetCurrentDateTime());

            if (count <= 40 && category == UserCategories.Standard)
            {
                return UserCategories.Standard;
            }
            if (count <= 80 && category != UserCategories.Gold)
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
            var usersInTraining = db.GetNonProcessedUsersInTrainingBeforeDate(dateTimeService.GetCurrentDateTime());
            foreach (var item in usersInTraining)
            {
                IUser user = db.GetUserData(item.Email);
                db.SetAlreadyProcessedFlag(item);
                if (user.NumberOfFreeSignUpsOnSeasonTicket == 0)
                {
                    db.SetZeroEntranceFlag(item, true);

                }
                else
                {
                    db.SetZeroEntranceFlag(item, false);
                }

                if (user.NumberOfFreeSignUpsOnSeasonTicket > 0)
                {
                    db.UpdateCountOfFreeSignUps(item.Email, -1);
                }
            }
        }

        public void PromptToConfirmUserAttendance()
        {
            var usersInTraining = db.GetAllUsersInTrainingWithZeroEntranceFlag();
            if (usersInTraining.Count == 0)
            {
                return;
            }
            var trainingsIDs = usersInTraining.Select(r => r.TrainingID).Distinct().ToList();

            var admins = db.GetAdmins();

            foreach (var item in trainingsIDs)
            {
                var users = usersInTraining.Where(r => r.TrainingID == item).Select(r => db.GetUserData(r.Email)).ToList();
                var trainingData = db.GetTrainingData(item);
                emailManager.SendConfirmUserAttendance(admins, users, item, trainingData.Time);
            }
            usersInTraining.Select(r =>
            {
                db.SetZeroEntranceFlag(r, false);
                return 0;
            }).ToList();

        }

        public bool SendNewPass(string email)
        {
            var user = db.GetUserData(email);
            if(user ==  null)
            {
            return false;
            }
            string pass = uidService.CreatePassword();
            string hash = hasher.HashPassword(pass);
            db.ChangePassword(email, hash);
            emailManager.SendPassRecoveryEmail(email, pass);
            return true;

        }

        public void SetRememberMe(string email, string cookieSeries, string cookieToken)
        {
            db.SetRememberMe(email, cookieSeries, cookieToken);
        }

        public ILoginResult VerifyRememberMe(string cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                return null;
            }
            var split = cookie.Split('=');

            if (split.Length != 2)
            {
                return null;
            }
            string series = split[0];
            string token = split[1];
            IUser user = db.LoadUser(series, token);
            if (user != null)
            {
                LoginResult result = new LoginResult();
                result.Category = user.Category;
                result.Email = user.Email;
                result.IsAdmin = user.IsAdmin;

                return result;
            }
            return null;
        }

        public void CancelRememberMe(string email)
        {
            db.CancelRememberMe(email);
        }
        public void CleanUpTokens()
        {
            db.CleanUpTokens();
        }
    }
}