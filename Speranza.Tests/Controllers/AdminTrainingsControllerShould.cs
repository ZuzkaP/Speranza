using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services.Interfaces;
using Moq;
using System.Web.Mvc;
using System.Web.SessionState;
using Speranza.Controllers;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AdminTrainingsControllerShould
    {
        private AdminTrainingsController controller;
        private Mock<IUserManager> userManager;
        private const string USER_EMAIL = "test";

        [TestMethod]
        public void ReturnToCalendar_When_ClickOnAdminUsers_And_UserIsNotLogin()
        {
            InitializeAccountController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(false);
            ActionResult result = controller.AdminTrainings();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void ReturnToCalendar_When_ClickOnAdminUsers_And_UserIsNotAdmin()
        {
            InitializeAccountController();
            controller.Session["IsAdmin"] = null;

            ActionResult result = controller.AdminTrainings();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void ShowView_When_ClickOnAdminUsers_And_UserIsAdmin()
        {
            InitializeAccountController();
            controller.Session["IsAdmin"] = true;

            ActionResult result = controller.AdminTrainings();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("AdminTrainings", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        private void InitializeAccountController()
        {
            userManager = new Mock<IUserManager>();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            controller = new AdminTrainingsController(userManager.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            controller.Session["Email"] = USER_EMAIL;
        }
    }
}
