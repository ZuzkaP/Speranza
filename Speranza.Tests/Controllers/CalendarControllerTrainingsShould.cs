using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using Moq;
using Speranza.Services.Interfaces;
using System.Web.Mvc;
using System.Web.SessionState;
using Speranza.Database;
using Speranza.Database.Data.Interfaces;
using Speranza.Models;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class CalendarControllerTrainingsShould
    {
        private CalendarController calendar;
        private Mock<IUserManager> userManager;
        private Mock<IDaysManager> daysManager;
        private Mock<IDateTimeService> dateTimeService;
        private const string INVALIDID = "invalidID";
        private Mock<IDatabaseGateway> db;
        private const string ID = "testID";
        private Mock<ITraining> training;
        private const string EMAIL = "testEmail";

        [TestMethod]
        public void ReturnToLogin_When_UserIsNotLoggedIn()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(false);
            RedirectToRouteResult result = calendar.SignUp(null);
            
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
           
        }


        [TestMethod]
        public void NotSignUp_When_TrainingIDDoesNotExist()
        {
            InitializeController();
            
            RedirectToRouteResult result = calendar.SignUp(INVALIDID);
            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.TrainingDoesNotExist, result.RouteValues["message"]);

        }

        [TestMethod]
        public void NotSignUp_When_TrainingIsFull()
        {
            InitializeController();
            training.SetupGet(r => r.RegisteredNumber).Returns(10);
            RedirectToRouteResult result = calendar.SignUp(ID);
            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.TrainingIsFull, result.RouteValues["message"]);
        }

        [TestMethod]
        public void SignUp_When_TrainingExistsAndIsNotFull()
        {
            InitializeController();
            RedirectToRouteResult result = calendar.SignUp(ID);
            db.Verify(r => r.AddUserToTraining(EMAIL, ID), Times.Once);
            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.SignUpSuccessful, result.RouteValues["message"]);


        }

        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            daysManager = new Mock<IDaysManager>();
            dateTimeService = new Mock<IDateTimeService>();
            training = new Mock<ITraining>();
            training.SetupGet(r => r.Capacity).Returns(10);
            training.SetupGet(r => r.RegisteredNumber).Returns(8);

            db = new Mock<IDatabaseGateway>();
            
            calendar = new CalendarController(db.Object,userManager.Object, daysManager.Object, dateTimeService.Object);
            
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            calendar.ControllerContext = new FakeControllerContext(calendar, sessionItems);
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(true);

            db.Setup(r => r.GetTrainingData(INVALIDID)).Returns((ITraining)null);
            db.Setup(r => r.GetTrainingData(ID)).Returns(training.Object);

            calendar.Session["Email"] = EMAIL;
        }
    }
}
