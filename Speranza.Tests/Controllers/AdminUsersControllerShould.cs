using System;
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
        private const string CATEGORY = "Gold";

        private const int INCREASECOUNT = 10;
        private const int DECREASECOUNT = -20;

        private const int CHANGEDCOUNT = 12;

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

        [TestMethod]
        public void SendCategoriesToUI()
        {
            InitializeAdminUsersController();

            ViewResult result = (ViewResult)controller.AdminUsers();

            AdminUsersModel model = (AdminUsersModel)result.Model;
            Assert.AreEqual(3, model.Categories.Count);
            Assert.AreEqual(UserCategories.Gold.ToString(), model.Categories[2]);
            Assert.AreEqual(UserCategories.Silver.ToString(), model.Categories[1]);
            Assert.AreEqual(UserCategories.Standard.ToString(), model.Categories[0]);
        }

        [TestMethod]
        public void NotChangeUserCategory_When_LoggedUserIsNotAdmin()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.UserCategory(USER_EMAIL, CATEGORY);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            userManager.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);

        }

        [TestMethod]
        public void NotChangeUserCategory_When_UserEmailIsEmpty()
        {
            InitializeAdminUsersController();

            JsonResult result = (JsonResult) controller.UserCategory(string.Empty, CATEGORY);

            Assert.AreEqual(string.Empty, result.Data);
            userManager.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
        }

        [TestMethod]
        public void NotChangeUserCategory_When_UserCategoryEmpty()
        {
            InitializeAdminUsersController();

            JsonResult result = (JsonResult)controller.UserCategory(USER_EMAIL, string.Empty);

            Assert.AreEqual(string.Empty, result.Data);
            userManager.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
        }

        [TestMethod]
        public void ChangeUserCategory()
        {
            InitializeAdminUsersController();

            JsonResult result =(JsonResult) controller.UserCategory(USER_EMAIL, CATEGORY);
            
            userManager.Verify(r => r.SetUserCategory(USER_EMAIL,UserCategories.Gold), Times.Once);
            Assert.AreEqual(UsersAdminMessages.SuccessfullyChangedCategory, ((UpdateCategoryModel)result.Data).Message);
            Assert.AreEqual(USER_EMAIL, ((UpdateCategoryModel)result.Data).Email);
            Assert.AreEqual(CATEGORY, ((UpdateCategoryModel)result.Data).Category);
        }


        [TestMethod]
        public void NotChangeCountOfFreeSignUps_When_LoggedUserIsNotAdmin()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.UpdateSignUpCount(USER_EMAIL, DECREASECOUNT);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            userManager.Verify(r => r.UpdateCountOfFreeSignUps(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void NotChangeCountOfFreeSignUps_When_UserEmailIsEmpty()
        {
            InitializeAdminUsersController();

            JsonResult result = (JsonResult)controller.UpdateSignUpCount(string.Empty, DECREASECOUNT);

            Assert.AreEqual(string.Empty, result.Data);
            userManager.Verify(r => r.UpdateCountOfFreeSignUps(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void IncreasedCountOfFreeSignUps()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.UpdateCountOfFreeSignUps(USER_EMAIL, INCREASECOUNT)).Returns(CHANGEDCOUNT);

            JsonResult result = (JsonResult)controller.UpdateSignUpCount(USER_EMAIL, INCREASECOUNT);

            userManager.Verify(r => r.UpdateCountOfFreeSignUps(USER_EMAIL, INCREASECOUNT), Times.Once);
            Assert.AreEqual(UsersAdminMessages.SuccessfullyIncreasedCountOfSignUps, ((UpdateCountOfSignUpsModel)result.Data).Message);
            Assert.AreEqual(USER_EMAIL, ((UpdateCountOfSignUpsModel)result.Data).Email);
            Assert.AreEqual(CHANGEDCOUNT, ((UpdateCountOfSignUpsModel)result.Data).AfterChangeNumberOfSignUps);
            Assert.AreEqual(INCREASECOUNT, ((UpdateCountOfSignUpsModel)result.Data).ChangeNumberOfSignUps);
        }

        [TestMethod]
        public void DecreasedCountOfFreeSignUps()
        {
            InitializeAdminUsersController();
            userManager.Setup(r => r.UpdateCountOfFreeSignUps(USER_EMAIL, DECREASECOUNT)).Returns(CHANGEDCOUNT);

            JsonResult result = (JsonResult)controller.UpdateSignUpCount(USER_EMAIL, DECREASECOUNT);

            userManager.Verify(r => r.UpdateCountOfFreeSignUps(USER_EMAIL, DECREASECOUNT), Times.Once);
            Assert.AreEqual(UsersAdminMessages.SuccessfullyDecreasedCountOfSignUps, ((UpdateCountOfSignUpsModel)result.Data).Message);
            Assert.AreEqual(USER_EMAIL, ((UpdateCountOfSignUpsModel)result.Data).Email);
            Assert.AreEqual(CHANGEDCOUNT, ((UpdateCountOfSignUpsModel)result.Data).AfterChangeNumberOfSignUps);
            Assert.AreEqual(DECREASECOUNT*(-1), ((UpdateCountOfSignUpsModel)result.Data).ChangeNumberOfSignUps);
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
