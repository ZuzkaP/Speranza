using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Controllers;
using Speranza.Models;
using Speranza.Services.Interfaces;
using Speranza.Services.Interfaces.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AdminTrainingsControllerCreatingNewTrainingShould 
    {
        private AdminFutureTrainingsController controller;
        private const string DESCRIPTION="test";
        private const string DATE = "01.01.2017";
        private const string TIME  ="05:00";
        private const string TRAINER ="Zuzka";
        private Mock<ITrainingsManager> trainingManager;
        private Mock<IUserManager> userManager;
        private Mock<IDateTimeService> dateTimeService;
        private readonly DateTime DATETIME= new DateTime(2017,01,01,12,00,00);
        private const string TRAINING_ID = "trainingID";
        private const int  CAPACITY = 10;
        private readonly DateTime DATETIME_IN_PAST = new DateTime(2016, 01, 01, 12, 00, 00);
        private const int UNCORRECT_CAPACITY = 0;

        [TestMethod]
        public void ReturnToCalendar_When_UserIsNotAdmin()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(false);

            ActionResult result = controller.CreateNewTraining(DATE, TIME, TRAINER, DESCRIPTION, CAPACITY);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateTraining_When_TrainerIsEmpty()
        {
            InitializeController();

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, string.Empty, DESCRIPTION,CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.NewTrainingNoTrainer, result.Data);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateTraining_When_DescriptionIsEmpty()
        {
            InitializeController();

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, TRAINER, string.Empty,CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.NewTrainingNoDescription, result.Data);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateTraining_When_CapacityIsLessThan1()
        {
            InitializeController();

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, TRAINER, DESCRIPTION, UNCORRECT_CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.TraininingCapacityCannotBeLessThanOne, result.Data);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }


        [TestMethod]
        public void NotCreateTraining_When_DateIsNotParsable()
        {
            InitializeController();
            dateTimeService.Setup(r => r.ParseDateTime(DATE, TIME)).Throws(new InvalidDateException());

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, TRAINER,DESCRIPTION,CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.NewTrainingDateInvalid, result.Data);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void NotCreateTraining_When_TimeIsNotParsable()
        {
            InitializeController();
            dateTimeService.Setup(r => r.ParseDateTime(DATE, TIME)).Throws(new InvalidTimeException());

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, TRAINER, DESCRIPTION,CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.NewTrainingTimeInvalid, result.Data);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }
        
        [TestMethod]
        public void NotCreateTraining_When_DateIsInPast()
        {
            InitializeController();
            dateTimeService.Setup(r => r.ParseDateTime(DATE, TIME)).Returns(DATETIME_IN_PAST);

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, TRAINER, DESCRIPTION, CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.NewTrainingDateInPast, result.Data);
            trainingManager.Verify(r => r.CreateNewTraining(It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void CreateTraining()
        {
            InitializeController();
            dateTimeService.Setup(r => r.ParseDateTime(DATE, TIME)).Returns(DATETIME);
            trainingManager.Setup(r => r.CreateNewTraining(DATETIME, TRAINER, DESCRIPTION,CAPACITY)).Returns(TRAINING_ID);

            JsonResult result = (JsonResult)controller.CreateNewTraining(DATE, TIME, TRAINER, DESCRIPTION,CAPACITY);

            Assert.AreEqual(AdminTrainingsMessages.NewTrainingSuccessfullyCreated, ((CreateTrainingModel) result.Data).Message);
            Assert.AreEqual(DATETIME.ToString("dd.MM.yyyy"), ((CreateTrainingModel) result.Data).Date);
            Assert.AreEqual(DATETIME.ToString("HH:mm"), ((CreateTrainingModel) result.Data).Time);
            Assert.AreEqual(TRAINING_ID, ((CreateTrainingModel) result.Data).TrainingID);
            Assert.AreEqual(TRAINER, ((CreateTrainingModel) result.Data).Trainer);
            Assert.AreEqual(DESCRIPTION, ((CreateTrainingModel) result.Data).Description);
            Assert.AreEqual(CAPACITY, ((CreateTrainingModel) result.Data).Capacity);
            trainingManager.Verify(r => r.CreateNewTraining(DATETIME, TRAINER, DESCRIPTION, CAPACITY),Times.Once);

        }

        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>(); 
            controller = new AdminFutureTrainingsController(userManager.Object, trainingManager.Object, dateTimeService.Object,null);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(DATETIME);

        }
    }
}
