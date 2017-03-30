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
using Speranza.Services.Interfaces.Exceptions;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AdminFutureTrainingsControllerShould
    {
        private AdminFutureTrainingsController controller;
        private Mock<IUserManager> userManager;
        private const string USER_EMAIL = "test";
        private Mock<ITrainingsManager> trainingManager;
        private const string TRAINING_ID = "testID";
        private const string TRAINER = "Miro";
        private const string TRAINING_DESCRIPTION = "description";
        private const int TRAINING_CAPACITY = 3;
        private const int TRAINING_CAPACITY_UNCORRECT = -1;
        private Mock<IDateTimeService> dateTimeService;
        private List<ITrainingForAdminModel> trainings;
        private Mock<IUserDataParser> userDataParser;

        private const int HOURS_LIMIT = 12;
        private const string USER_DATA = "user data";
        private const int DEFAULT_PAGE_SIZE = 20;
        private const int PAGE = 5;
        private const int CHANGED_PAGE_SIZE = 40;
        private readonly DateTime CURRENT_DATE = new DateTime(100000);

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
            trainingManager.Setup(r => r.GetFutureTrainings(0, DEFAULT_PAGE_SIZE)).Returns(trainings);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(trainings, model.Trainings);
            Assert.AreEqual(DEFAULT_PAGE_SIZE, model.PageSize);
        }

        [TestMethod]
        public void LoadUsersFromDB_And_SendToUI()
        {
            InitializeAdminTrainingsController();
            IList<IUserForTrainingDetailModel> users = new List<IUserForTrainingDetailModel>();
            userManager.Setup(r => r.GetAllUsersForTrainingDetails()).Returns(users);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(users, model.Users);
        }
        
        [TestMethod]
        public void ReturnCorrectNumberOfPages()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetFutureTrainingsCount()).Returns(25);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(2, model.PagesCount);

        }

        [TestMethod]
        public void LoadSignOffLimitFromDB_And_SendToUI()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetSignOffLimit()).Returns(HOURS_LIMIT);

            ViewResult result = (ViewResult)controller.AdminTrainings();
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(HOURS_LIMIT, model.SignOffLimit);
        }

        [TestMethod]
        public void ShowPageWithChangedPageSize_When_ItWasChangedByUser()
        {
            InitializeAdminTrainingsController();
            IList<ITrainingForAdminModel> trainings = new List<ITrainingForAdminModel>();
            trainingManager.Setup(r => r.GetFutureTrainings(0, CHANGED_PAGE_SIZE)).Returns(trainings);

            ViewResult result = (ViewResult)controller.AdminTrainings(CHANGED_PAGE_SIZE);
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(trainings, model.Trainings);
            Assert.AreEqual(CHANGED_PAGE_SIZE, model.PageSize);
        }

        [TestMethod]
        public void ReturnCorrectNumberOfPages_When_PageSizeWasChanged()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetFutureTrainingsCount()).Returns(50);

            ViewResult result = (ViewResult)controller.AdminTrainings(CHANGED_PAGE_SIZE);
            AdminTrainingsModel model = (AdminTrainingsModel)result.Model;

            Assert.AreEqual(2, model.PagesCount);
        }

        [TestMethod]
        public void ReturnToCalendar_When_ChangingTrainer_And_UserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.ChangeTrainer(TRAINING_ID,TRAINER);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.SetTrainer(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [TestMethod]
        public void NotToBeSet_When_ChangingTrainer_And_TrainerIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainer(TRAINING_ID, string.Empty);
          
            Assert.AreEqual(string.Empty, result.Data);
            trainingManager.Verify(r => r.SetTrainer(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void NotToBeSet_When_ChangingTrainer_And_TrainingIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainer(string.Empty, TRAINER);

            Assert.AreEqual(string.Empty, result.Data);
            trainingManager.Verify(r => r.SetTrainer(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ChangeTrainer()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainer(TRAINING_ID, TRAINER);

            Assert.AreEqual(AdminTrainingsMessages.TrainerWasSuccessfullyChanged, result.Data);
            trainingManager.Verify(r => r.SetTrainer(TRAINING_ID, TRAINER), Times.Once);
        }

        [TestMethod]
        public void ReturnToCalendar_When_ChangingTrainingDescription_And_UserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.ChangeTrainingDescription(TRAINING_ID, TRAINING_DESCRIPTION);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.SetTrainingDescription(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        [TestMethod]
        public void NotToBeSet_When_ChangingTrainingDescription_And_TrainingIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainingDescription(string.Empty, TRAINING_DESCRIPTION);

            Assert.AreEqual(string.Empty, result.Data);
            trainingManager.Verify(r => r.SetTrainingDescription(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void NotToBeSet_When_ChangingTrainingCapacity_And_TrainingCapacityIsLessThanZero_When_ChangingTrainingDescription_And_TrainingDescriptionIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainingDescription(TRAINING_ID, string.Empty);

            Assert.AreEqual(string.Empty, result.Data);
            trainingManager.Verify(r => r.SetTrainingDescription(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void ChangeTrainingDescription()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainingDescription(TRAINING_ID, TRAINING_DESCRIPTION);

            Assert.AreEqual(AdminTrainingsMessages.TraininingDescriptionWasSuccessfullyChanged, result.Data);
            trainingManager.Verify(r => r.SetTrainingDescription(TRAINING_ID, TRAINING_DESCRIPTION), Times.Once);
        }



        [TestMethod]
        public void ReturnToCalendar_When_ChangingTrainingCapacity_And_UserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.ChangeTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.SetTrainingCapacity(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void NotToBeSet_When_ChangingTrainingCapacity_And_TrainingCapacityIsLessThanOne()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY_UNCORRECT);

            Assert.AreEqual(AdminTrainingsMessages.TraininingCapacityCannotBeLessThanOne, result.Data);
            trainingManager.Verify(r => r.SetTrainingCapacity(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }
        [TestMethod]
        public void NotToBeSet_ChangingTrainingCapacity_When_TrainingIDIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainingCapacity(string.Empty, TRAINING_CAPACITY_UNCORRECT);

            Assert.AreEqual(string.Empty, result.Data);
            trainingManager.Verify(r => r.SetTrainingCapacity(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }


        [TestMethod]
        public void NotChangeCapacityLowerThanCountOfSignedUpUsers()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetAllUsersInTraining(TRAINING_ID).Count).Returns(6);

            JsonResult result = (JsonResult)controller.ChangeTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.TraininingCapacityLowerThanCountOfSignedUpUsers, result.Data);
            trainingManager.Verify(r => r.SetTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY), Times.Never);
        }

        [TestMethod]
        public void ChangeTrainingCapacity()
        {
            InitializeAdminTrainingsController();
            trainingManager.Setup(r => r.GetAllUsersInTraining(TRAINING_ID).Count).Returns(1);

            JsonResult result = (JsonResult)controller.ChangeTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.TraininingCapacityWasSuccessfullyChanged, result.Data);
            trainingManager.Verify(r => r.SetTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY), Times.Once);
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
            Assert.IsNotNull( model.Users);
            Assert.AreEqual(TRAINING_ID, model.TrainingID);
        }

        [TestMethod]
      
        public void NotCancelTraining_WhenTrainingIDIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.CancelTraining(string.Empty);

            Assert.AreEqual(AdminTrainingsMessages.TrainingIDInvalid, result.Data);
            trainingManager.Verify(r => r.CancelTraining(It.IsAny<string>()), Times.Never);
        }
             
       [TestMethod]
        public void NotCancelTraining_WhenUserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.CancelTraining(TRAINING_ID);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CancelTraining(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void CancelTraining()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.CancelTraining(TRAINING_ID);
           
            Assert.AreEqual(AdminTrainingsMessages.TrainingWasCanceled, result.Data);
            trainingManager.Verify(r => r.CancelTraining(TRAINING_ID), Times.Once);
        }

        [TestMethod]
        public void NotChangeSignOffLimit_WhenUserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.SetSignOffLimit(HOURS_LIMIT);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.SetSignOffLimit(It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void ChangeSignOffLimit()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.SetSignOffLimit(HOURS_LIMIT);

            Assert.AreEqual(AdminTrainingsMessages.SignOffLimitWasChanged, result.Data);
            trainingManager.Verify(r => r.SetSignOffLimit(HOURS_LIMIT), Times.Once);
        }

        [TestMethod]
        public void NotAddUserToTraining_When_UserIsNotAdmin()
        {
            InitializeAdminTrainingsController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.AddUserToTraining(TRAINING_ID, USER_DATA);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<DateTime>()), Times.Never);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Never);
        }
        [TestMethod]
        public void NotAddUserToTraining_When_UserDataAreNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.AddUserToTraining(TRAINING_ID, null);

            Assert.AreEqual(CalendarMessages.UserDoesNotExist, result.Data);
            userDataParser.Verify(r => r.ParseData(It.IsAny<string>()), Times.Never);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Never);

        }

        [TestMethod]
        public void NotAddUserToTraining_When_UserDataDoNotContainEmail()
        {
            InitializeAdminTrainingsController();
            userDataParser.Setup(r => r.ParseData(USER_DATA)).Returns((string)null);

            JsonResult result =(JsonResult)controller.AddUserToTraining(TRAINING_ID, USER_DATA);
            
            Assert.AreEqual(CalendarMessages.UserDoesNotExist, result.Data);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<bool>()), Times.Never);
        }

        [TestMethod]
        public void SetAddedUserDataToUi_When_UserSuccessfullyAddedToTraining()
        {
            InitializeAdminTrainingsController();
            userDataParser.Setup(r => r.ParseData(USER_DATA)).Returns(USER_EMAIL);
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(CURRENT_DATE);
            trainingManager.Setup(r => r.AddUserToTraining(USER_EMAIL, TRAINING_ID, CURRENT_DATE,true)).Returns(CalendarMessages.SignUpSuccessful);
            var user = new Mock<IUserForTrainingDetailModel>();
            userManager.Setup(r => r.GetAddedUserData(USER_EMAIL)).Returns(user.Object);

            JsonResult result = (JsonResult) controller.AddUserToTraining(TRAINING_ID, USER_DATA);

            IUserForTrainingDetailModel model = (IUserForTrainingDetailModel)result.Data;
            Assert.AreEqual(user.Object, model);
            trainingManager.Verify(r => r.AddUserToTraining(USER_EMAIL, TRAINING_ID, CURRENT_DATE, true), Times.Once);
            trainingManager.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);

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

            ActionResult result = controller.ShowTrainingsPage(PAGE,DEFAULT_PAGE_SIZE);

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

            trainingManager.Verify(r => r.GetFutureTrainings(80, 100), Times.Once);

            TrainingsPageModel model = (TrainingsPageModel)result.Model;
            Assert.AreEqual("TrainingsPage", result.ViewName);
            Assert.AreEqual(trainings, model.Trainings);
        }

        private void PrepareTrainingsForPage()
        {
            trainings = new List<ITrainingForAdminModel>();
            trainingManager.Setup(r => r.GetFutureTrainings(80, 100)).Returns(trainings);
        }

        private void InitializeAdminTrainingsController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            userDataParser = new Mock<IUserDataParser>();
            controller = new AdminFutureTrainingsController(userManager.Object, trainingManager.Object, dateTimeService.Object, userDataParser.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            controller.Session["Email"] = USER_EMAIL;
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);
        }
    }
}
