﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services.Interfaces;
using Speranza.Controllers;
using System.Web.SessionState;
using Speranza.Services;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class UserManagerShould
    {
        private SessionStateItemCollection collection;
        private UserManager manager;

        [TestMethod]
        public void ReturnFalse_When_SessionIsEmpty()
        {
            InitializeManager();
           Assert.IsFalse(manager.IsUserLoggedIn(collection));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailInSessionDoesNotExist()
        {
            InitializeManager();
            collection["notEmail"] = "test";
            Assert.IsFalse(manager.IsUserLoggedIn(collection));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailIsEmpty()
        {
            InitializeManager();
            collection["Email"] = "";
            Assert.IsFalse(manager.IsUserLoggedIn(collection));
            
        }

        [TestMethod]
        public void ReturnTrue_When_EmailSessionDoesExist()
        {
            InitializeManager();
            collection["Email"] = "test";
            Assert.IsTrue(manager.IsUserLoggedIn(collection));
        }


        [TestMethod]
        public void ReturnStandardCategory_When_CategoryIsNotInSession()
        {
            InitializeManager();
            Assert.AreEqual(UserCategories.Standard,manager.GetUserCategory(collection));

        }

        [TestMethod]
        public void ReturnTheRightCategory_When_CategoryIsInSession()
        {
            InitializeManager();
            collection["Category"] = UserCategories.Gold;
            Assert.AreEqual(UserCategories.Gold, manager.GetUserCategory(collection));
        }

        private void InitializeManager()
        {
            manager = new UserManager();
            collection = new SessionStateItemCollection();
        }
    }
}