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

namespace Speranza.Services
{
    public class UserManager : IUserManager
    {
        private IDatabaseGateway db;
        private IModelFactory factory;

        public UserManager(IDatabaseGateway db, IModelFactory factory)
        {
            this.db = db;
            this.factory = factory;
        }

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
            throw new NotImplementedException();
        }
    }
}