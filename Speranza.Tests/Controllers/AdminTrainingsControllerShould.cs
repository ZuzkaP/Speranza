using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services.Interfaces;
using Moq;
using System.Web.Mvc;
using System.Web.SessionState;
using Speranza.Controllers;
using Speranza.Models.Interfaces;
using System.Collections.Generic;
using Speranza.Models;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AdminTrainingsControllerShould
    {
        private AdminTrainingsController controller;
        private Mock<IUserManager> userManager;
        private const string USER_EMAIL = "test";
        private Mock<ITrainingsManager> trainingManager;

        [TestMethod]
        public void ReturnToCalendar_When_ClickOnAdminUsers_And_UserIsNotLogin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(false);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.AdminTrainings();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void ReturnToCalendar_When_ClickOnAdminUsers_And_UserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.AdminTrainings();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void ShowView_When_ClickOnAdminUsers_And_UserIsAdmin()
        {
            InitializeAdminTrainingsController();

            ActionResult result = controller.AdminTrainings();

            Assert.AreEqual("AdminTrainings", ((ViewResult)result).ViewName);
        }
      
        [TestMethod]
        public void LoadTrainingsFromDB_And_SendTOUI()
        {
            InitializeAdminTrainingsController();
            IList<ITrainingForAdminModel> trainings = new List<ITrainingForAdminModel>();
            trainingManager.Setup(r => r.GetAllTrainingsForAdmin()).Returns(trainings);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(trainings, model.Trainings);
        }

        private void InitializeAdminTrainingsController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            controller = new AdminTrainingsController(userManager.Object,trainingManager.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            controller.Session["Email"] = USER_EMAIL;
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);
        }
    }
}
