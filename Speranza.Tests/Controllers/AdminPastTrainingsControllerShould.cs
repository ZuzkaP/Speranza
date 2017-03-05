using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services.Interfaces;
using Moq;
using Speranza.Controllers;
using System.Web.SessionState;
using System.Web.Mvc;
using Speranza.Models.Interfaces;
using System.Collections.Generic;
using Speranza.Models;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AdminPastTrainingsControllerShould
    {
        private Mock<IUserManager> userManager;
        private Mock<ITrainingsManager> trainingManager;
        private Mock<IDateTimeService> dateTimeService;
        private AdminPastTrainingsController controller;

        private const string USER_EMAIL = "test";
        private const int DEFAULT_PAGE_SIZE = 20;
        private const int PAGE = 5;
        private const int CHANGED_PAGE_SIZE = 40;
        private const string TRAINING_ID = "testID";
        private List<ITrainingForAdminModel> trainings;

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
        public void LoadTrainingsFromDB_And_SendToUI()
        {
            InitializeAdminTrainingsController();
            IList<ITrainingForAdminModel> trainings = new List<ITrainingForAdminModel>();
            trainingManager.Setup(r => r.GetPastTrainings(0, DEFAULT_PAGE_SIZE)).Returns(trainings);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(trainings, model.Trainings);
            Assert.AreEqual(DEFAULT_PAGE_SIZE, model.PageSize);
        }

        [TestMethod]
        public void ReturnCorrectNumberOfPages()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetPastTrainingsCount()).Returns(25);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(2, model.PagesCount);
        }

        [TestMethod]
        public void ShowPageWithChangedPageSize_When_ItWasChangedByUser()
        {
            InitializeAdminTrainingsController();
            IList<ITrainingForAdminModel> trainings = new List<ITrainingForAdminModel>();
            trainingManager.Setup(r => r.GetPastTrainings(0, CHANGED_PAGE_SIZE)).Returns(trainings);

            ViewResult result = (ViewResult)controller.AdminTrainings(CHANGED_PAGE_SIZE);
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(trainings, model.Trainings);
            Assert.AreEqual(CHANGED_PAGE_SIZE, model.PageSize);
        }

        [TestMethod]
        public void ReturnCorrectNumberOfPages_When_PageSizeWasChanged()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetPastTrainingsCount()).Returns(50);

            ViewResult result = (ViewResult)controller.AdminTrainings(CHANGED_PAGE_SIZE);
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(2, model.PagesCount);
        }

        [TestMethod]
        public void ShowFirstPageOfTrainingsByDefault()
        {
            InitializeAdminTrainingsController();

            ViewResult result = (ViewResult)controller.AdminTrainings();

            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;
            Assert.AreEqual(1, model.PageNumber);
        }

        [TestMethod]
        public void NotChangeTrainingsPage_When_UserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.ShowTrainingsPage(PAGE, DEFAULT_PAGE_SIZE);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void ShowCorrectPage()
        {
            InitializeAdminTrainingsController();
            PrepareTrainingsForPage();

            PartialViewResult result = (PartialViewResult)controller.ShowTrainingsPage(PAGE, DEFAULT_PAGE_SIZE);

            trainingManager.Verify(r => r.GetPastTrainings(80, 100), Times.Once);

            TrainingsPageModel model = (TrainingsPageModel)result.Model;
            Assert.AreEqual("TrainingsPage", result.ViewName);
            Assert.AreEqual(trainings, model.Trainings);
        }

        [TestMethod]
        public void NotShowTrainingsDetails_When_TrainingIsEmpty()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.TrainingDetails(string.Empty);

            Assert.AreEqual(string.Empty, result.Data);
            trainingManager.Verify(r => r.GetAllUsersInTraining(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void NotShowTrainingsDetails_When_LoggedUserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.TrainingDetails(TRAINING_ID);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.GetAllUsersInTraining(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ShowTrainingsDetails()
        {
            InitializeAdminTrainingsController();
            IList<IUserForTrainingDetailModel> users = new List<IUserForTrainingDetailModel>();
            trainingManager.Setup(r => r.GetAllUsersInTraining(TRAINING_ID)).Returns(users);

            PartialViewResult result = (PartialViewResult)controller.TrainingDetails(TRAINING_ID);

            UsersInTrainingModel model = (UsersInTrainingModel)result.Model;
            Assert.AreEqual("UsersInTraining", result.ViewName);
            Assert.AreEqual(users, model.Users);
            Assert.AreEqual(TRAINING_ID, model.TrainingID);
        }


        private void PrepareTrainingsForPage()
        {
            trainings = new List<ITrainingForAdminModel>();
            trainingManager.Setup(r => r.GetPastTrainings(80, 100)).Returns(trainings);
        }

        private void InitializeAdminTrainingsController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            controller = new AdminPastTrainingsController(userManager.Object, trainingManager.Object, dateTimeService.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            controller.Session["Email"] = USER_EMAIL;
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);
        }
    }
}
