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


        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            daysManager = new Mock<IDaysManager>();
            dateTimeService = new Mock<IDateTimeService>();
            db = new Mock<IDatabaseGateway>();
            db.Setup(r => r.GetTrainingData(INVALIDID)).Returns((ITraining)null);
            calendar = new CalendarController(userManager.Object, daysManager.Object, dateTimeService.Object);
            
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            calendar.ControllerContext = new FakeControllerContext(calendar, sessionItems);
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(true);

        }
    }
}
