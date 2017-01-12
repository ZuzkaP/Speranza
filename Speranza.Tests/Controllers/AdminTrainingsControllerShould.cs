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
    public class AdminTrainingsControllerShould
    {
        private AdminTrainingsController controller;
        private Mock<IUserManager> userManager;
        private const string USER_EMAIL = "test";
        private Mock<ITrainingsManager> trainingManager;
        private const string TRAINING_ID = "testID";
        private const string TRAINER = "Miro";
        private const string TRAINING_DESCRIPTION = "description";
        private const int TRAINING_CAPACITY = 8;
        private const int TRAINING_CAPACITY_UNCORRECT = -1;
        private Mock<IDateTimeService> dateTimeService;
        private Mock<ITrainingForAdminModel> trainingModel;
        private List<ITrainingForAdminModel> trainings;
        private Mock<IAdminTrainingsModel> model;

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
        public void NotToBeSet_When_ChangingTrainingCapacity_And_TrainingCapacityIsLessThanZero()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.ChangeTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY_UNCORRECT);

            Assert.AreEqual(AdminTrainingsMessages.TraininingCapacityCannotBeLessThanZero, result.Data);
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
        public void ChangeTrainingCapacity()
        {
            InitializeAdminTrainingsController();

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
            Assert.AreEqual(users, model.Users);
            Assert.AreEqual(TRAINING_ID, model.TrainingID);
        }

        [TestMethod]
        [ExpectedException(typeof(IInvalidTrainingIDException))]
        public void NotCancelTraining_WhenTrainingIDIsNull()
        {
            InitializeAdminTrainingsController();

            JsonResult result = (JsonResult)controller.CancelTraining(string.Empty);

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
           /// PrepareModelWithTwoTrainings();

            JsonResult result = (JsonResult)controller.CancelTraining(TRAINING_ID);

            //AdminTrainingsModel model = (UsersInTrainingModel)result.Model;
            //Assert.AreEqual("UsersInTraining", result.ViewName);
            //Assert.AreEqual(users, model.Users);
            //Assert.AreEqual(TRAINING_ID, model.TrainingID);
           
            Assert.AreEqual(AdminTrainingsMessages.TrainingWasCanceled, result.Data);
            trainingManager.Verify(r => r.CancelTraining(TRAINING_ID), Times.Once);
        }

        //private void PrepareModelWithTwoTrainings()
        //{
        //    trainingModel = new Mock<ITrainingForAdminModel>();
        //    trainingModel.SetupGet(r => r.ID).Returns(TRAINING_ID);
        //    trainings = new List<ITrainingForAdminModel>();
        //    trainings.Add(trainingModel.Object);
        //    model = new Mock<IAdminTrainingsModel>();
        //    model.SetupGet(r => r.Trainings).Returns(trainings);
        //}

        private void InitializeAdminTrainingsController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            controller = new AdminTrainingsController(userManager.Object, trainingManager.Object, dateTimeService.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            controller.Session["Email"] = USER_EMAIL;
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);

        }
    }
}
