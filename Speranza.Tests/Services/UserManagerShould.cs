﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services.Interfaces;
using Speranza.Controllers;
using System.Web.SessionState;
using Speranza.Services;
using Speranza.Database.Data.Interfaces;
using Speranza.Tests.Controllers;
using Speranza.Models.Interfaces;
using System.Collections.Generic;
using Moq;
using Speranza.Database;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class UserManagerShould
    {
        private SessionStateItemCollection collection;
        private UserManager manager;
        private FakeControllerContext context;
        private Mock<IDatabaseGateway> db;
        private Mock<IModelFactory> factory;
        private Mock<IUserForAdminModel> userModel1;
        private Mock<IUserForAdminModel> userModel2;
        private const string EMAIL = "test";
        private readonly int INCRESENUMBEROFSIGNUPS = 10;
        private readonly int DECREASENUMBEROFSIGNUPS = -10;

        [TestMethod]
        public void ReturnFalse_When_SessionIsEmpty()
        {
            InitializeUserManager();
           Assert.IsFalse(manager.IsUserLoggedIn(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailInSessionDoesNotExist()
        {
            InitializeUserManager();
            collection["notEmail"] = "test";
            Assert.IsFalse(manager.IsUserLoggedIn(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailIsEmpty()
        {
            InitializeUserManager();
            collection["Email"] = "";
            Assert.IsFalse(manager.IsUserLoggedIn(context.HttpContext.Session));
            
        }

        [TestMethod]
        public void ReturnTrue_When_EmailSessionDoesExist()
        {
            InitializeUserManager();
            collection["Email"] = "test";
            Assert.IsTrue(manager.IsUserLoggedIn(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_AdminSessionIsNull()
        {
            InitializeUserManager();
            collection["IsAdmin"] = null;
            Assert.IsFalse(manager.IsUserAdmin(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_AdminSessionIsFalse()
        {
            InitializeUserManager();
            collection["IsAdmin"] = false;
            Assert.IsFalse(manager.IsUserAdmin(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnTrue_When_AdminSessionIsTrue()
        {
            InitializeUserManager();
            collection["IsAdmin"] = true;
            Assert.IsTrue(manager.IsUserAdmin(context.HttpContext.Session));
        }
        [TestMethod]
        public void ReturnStandardCategory_When_CategoryIsNotInSession()
        {
            InitializeUserManager();
            Assert.AreEqual(UserCategories.Standard,manager.GetUserCategory(context.HttpContext.Session));

        }

        [TestMethod]
        public void ReturnTheRightCategory_When_CategoryIsInSession()
        {
            InitializeUserManager();
            collection["Category"] = UserCategories.Gold;
            Assert.AreEqual(UserCategories.Gold, manager.GetUserCategory(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnEmptyUsersList_When_NoUserIsInDb()
        {
            InitializeUserManager();
            PrepareDBWithNoUser();

            IList<IUserForAdminModel> usersList =manager.GetAllUsersForAdmin();

            Assert.IsNotNull(usersList);
            Assert.AreEqual(0, usersList.Count);

        }

        [TestMethod]
        public void ReturnUsersList_When_UsersExistInDb()
        {
            InitializeUserManager();
            PrepareDBWithTwoUsers();

            IList<IUserForAdminModel> usersList = manager.GetAllUsersForAdmin();

            Assert.IsNotNull(usersList);
            Assert.AreEqual(2, usersList.Count);
            Assert.AreEqual(userModel1.Object, usersList[0]);
            Assert.AreEqual(userModel2.Object, usersList[1]);

        }

        [TestMethod]
        public void ChangeCountOfFreeSignUps_When_AdminChangesTheValue()
        {
            InitializeUserManager();
            db.Setup(r => r.UpdateCountOfFreeSignUps(EMAIL, INCRESENUMBEROFSIGNUPS)).Returns(25);
           
            int updatedCount = manager.UpdateCountOfFreeSignUps(EMAIL, INCRESENUMBEROFSIGNUPS);

            db.Verify(r => r.UpdateCountOfFreeSignUps(EMAIL,INCRESENUMBEROFSIGNUPS), Times.Once);
            Assert.AreEqual(25, updatedCount);
        }
        
        private void PrepareDBWithTwoUsers()
        {
            var user1 = new Mock<IUser>();
            var user2 = new Mock<IUser>();
            var usersList = new List<IUser>() { user1.Object, user2.Object};
            db.Setup(r => r.GetAllUsers()).Returns(usersList);

            userModel1 = new Mock<IUserForAdminModel>();
            userModel2 = new Mock<IUserForAdminModel>();

            factory.Setup(r => r.CreateUserForAdminModel(user1.Object)).Returns(userModel1.Object);
            factory.Setup(r => r.CreateUserForAdminModel(user2.Object)).Returns(userModel2.Object);
        }


        [TestMethod]
        public void SetAdminRoleToUser()
        {
            InitializeUserManager();

            manager.SetUserRoleToAdmin(EMAIL, true);

            db.Verify(r => r.SetAdminRole(EMAIL, true), Times.Once);
        }

        [TestMethod]
        public void SetCategoryToUserByAdmin()
        {
            InitializeUserManager();

            manager.SetUserCategory(EMAIL, UserCategories.Gold);

            db.Verify(r => r.SetUserCategory(EMAIL, UserCategories.Gold), Times.Once);
        }
        
        private void PrepareDBWithNoUser()
        {
            db.Setup(r => r.GetAllUsers()).Returns(new List<IUser>());
        }

        private void InitializeUserManager()
        {
            factory = new Mock<IModelFactory>();
            db = new Mock<IDatabaseGateway>();
            manager = new UserManager(db.Object, factory.Object);
            collection = new SessionStateItemCollection();
            context = new FakeControllerContext(null, collection);

        }
    }
}
