﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Speranza.Controllers;
using Speranza.Services.Interfaces;
using Moq;
using System.Web.SessionState;
using Speranza.Models;
using Speranza.Database.Data.Interfaces;
using System.Collections.Generic;
using Speranza.Database;
using Speranza.Models.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AdminUsersControllerShould
    {
        private AdminUsersController controller;
        private Mock<IUserManager> userManager;
        private const string USER_EMAIL = "test";
        private Mock<IDatabaseGateway> db;


        [TestMethod]
        public void ReturnToCalendar_When_ClickOnAdminUsers_And_UserIsNotLogin()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(false);
            ActionResult result = controller.AdminUsers();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);

        }

        [TestMethod]
        public void ReturnToCalendar_When_ClickOnAdminUsers_And_UserIsNotAdmin()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.AdminUsers();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
         }

        [TestMethod]
        public void ShowView_When_ClickOnAdminUsers_And_UserIsAdmin()
        {
            InitializeAdminUsersController();
            controller.Session["IsAdmin"] = true;

            ActionResult result = controller.AdminUsers();
            
           Assert.AreEqual("AdminUsers", ((ViewResult)result).ViewName);
         }

        [TestMethod]
        public void LoadUsersDatafromDBAndSendToUI_When_AdminIsLoggedIn()
        {
            InitializeAdminUsersController();
            IList<IUserForAdminModel> users = new List<IUserForAdminModel>();
            userManager.Setup(r => r.GetAllUsersForAdmin()).Returns(users);
            
            ViewResult result = (ViewResult)controller.AdminUsers();
            AdminUsersModel model = (AdminUsersModel)result.Model;

            Assert.AreEqual(users, model.Users);
        }

        [TestMethod]
        public void NotChangeIsAdminRoleForUser_When_UserEmailIsEmpty()
        {
            InitializeAdminUsersController();

            JsonResult result =(JsonResult) controller.ToggleAdmin(null,true);

            Assert.AreEqual(string.Empty, result.Data);
            userManager.Verify(r => r.SetUserRoleToAdmin(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);

        }


        [TestMethod]
        public void NotChangeIsAdminRoleForUser_When_LoggedUserIsNotAdmin()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.ToggleAdmin(USER_EMAIL, true);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            userManager.Verify(r => r.SetUserRoleToAdmin(It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void SetAdminRoleForUser_When_LoggedUserIsAdmin()
        {
            InitializeAdminUsersController();

            JsonResult result = (JsonResult)controller.ToggleAdmin(USER_EMAIL, true);

            userManager.Verify(r => r.SetUserRoleToAdmin(USER_EMAIL, true), Times.Once);
            Assert.AreEqual(UsersAdminMessages.SuccessfullySetAdminRole, ((ToggleAdminModel)result.Data).Message);
            Assert.AreEqual(USER_EMAIL, ((ToggleAdminModel)result.Data).Email);
        }

        [TestMethod]
        public void ClearAdminRoleForUser_When_LoggedUserIsAdmin()
        {
            InitializeAdminUsersController();

            JsonResult result = (JsonResult)controller.ToggleAdmin(USER_EMAIL, false);

            userManager.Verify(r => r.SetUserRoleToAdmin(USER_EMAIL, false), Times.Once);

            Assert.AreEqual(UsersAdminMessages.SuccessfullyClearAdminRole,((ToggleAdminModel) result.Data).Message);
            Assert.AreEqual(USER_EMAIL, ((ToggleAdminModel)result.Data).Email);
        }

        private void InitializeAdminUsersController()
        {
            userManager = new Mock<IUserManager>();
            controller = new AdminUsersController(userManager.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            controller.Session["Email"] = USER_EMAIL;
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);
        }
    }
}
