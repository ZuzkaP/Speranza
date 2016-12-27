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
using Speranza.Models.Interfaces;

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
        private Mock<ITrainingsManager> trainingManager;
        private readonly DateTime CURRENT_TIME = new DateTime(2017,1,1,10,00,00);

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
            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);

            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.TrainingDoesNotExist, calendar.Session["Message"]);

        }

        [TestMethod]
        public void NotSignUp_When_TrainingIsFull()
        {
            InitializeController();
            training.SetupGet(r => r.RegisteredNumber).Returns(10);

            RedirectToRouteResult result = calendar.SignUp(ID);

            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(),It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.TrainingIsFull, calendar.Session["Message"]);
        }

        [TestMethod]
        public void SignUp_When_TrainingExistsAndIsNotFull()
        {
            InitializeController();

            RedirectToRouteResult result = calendar.SignUp(ID);

            db.Verify(r => r.AddUserToTraining(EMAIL, ID,CURRENT_TIME), Times.Once);
            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.SignUpSuccessful, calendar.Session["Message"]);
        }
        
        [TestMethod]
        public void NotSignUp_When_UserIsAlreadySignedUp()
        {
            InitializeController();
            db.Setup(r => r.IsUserAlreadySignedUpInTraining(EMAIL, ID)).Returns(true);

            RedirectToRouteResult result = calendar.SignUp(ID);
           
            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.UserAlreadySignedUp, calendar.Session["Message"]);
        }
        
        [TestMethod]
        public void SignOff()
        {
            InitializeController();

            RedirectToRouteResult result = calendar.SignOff(ID);

            db.Verify(r => r.RemoveUserFromTraining(EMAIL, ID));
            Assert.AreEqual("Calendar", result.RouteValues["action"]);
            Assert.AreEqual(CalendarMessages.UserWasSignedOff, calendar.Session["Message"]);
        }

        [TestMethod]
        public void SignOff_And_ReturnToUserProfile_When_CalledFromUserProfile()
        {
            InitializeController();
            ((FakeControllerContext.FakeHttpContext)calendar.ControllerContext.RequestContext.HttpContext).RequestMock.SetupGet(r => r.UrlReferrer).Returns(new Uri("http://localhost/Accounts/UserProfile"));
                
            RedirectToRouteResult result = calendar.SignOff(ID);

            db.Verify(r => r.RemoveUserFromTraining(EMAIL, ID));
            Assert.AreEqual("UserProfile", result.RouteValues["action"]);
            Assert.AreEqual("Accounts", result.RouteValues["controller"]);
            Assert.AreEqual(CalendarMessages.UserWasSignedOff, calendar.Session["Message"]);
        }

        [TestMethod]
        public void SendSignedOffTrainingToUi_When_SigningOff()
        {
            InitializeController();
            Mock<ITrainingModel> trainingModel = new Mock<ITrainingModel>();
            trainingManager.Setup(r => r.CreateModel(training.Object)).Returns(trainingModel.Object);

            RedirectToRouteResult result = calendar.SignOff(ID);
           
            Assert.AreEqual(trainingModel.Object, calendar.Session["Training"]);
        }

        [TestMethod]
        public void SendSignedUpTrainingToUi_When_Signup()
        {
            InitializeController();
            Mock<ITrainingModel> trainingModel = new Mock<ITrainingModel>();
            trainingManager.Setup(r => r.CreateModel(training.Object)).Returns(trainingModel.Object);

            RedirectToRouteResult result = calendar.SignUp(ID);

            Assert.AreEqual(trainingModel.Object, calendar.Session["Training"]);
        }

        [TestMethod]
        public void ReturnToLogin_When_UserIsNotLoggedIn_And_SigningOff()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(false);

            RedirectToRouteResult result = calendar.SignOff(null);

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);

        }

        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            daysManager = new Mock<IDaysManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            training = new Mock<ITraining>();

            training.SetupGet(r => r.Capacity).Returns(10);
            training.SetupGet(r => r.RegisteredNumber).Returns(8);
            training.SetupGet(r => r.Time).Returns(new DateTime(2017,1,9,10,00,00));

            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(CURRENT_TIME);

            db = new Mock<IDatabaseGateway>();
            
            calendar = new CalendarController(db.Object,userManager.Object, daysManager.Object, dateTimeService.Object,trainingManager.Object);
            
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            calendar.ControllerContext = new FakeControllerContext(calendar, sessionItems);
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(true);

            db.Setup(r => r.GetTrainingData(INVALIDID)).Returns((ITraining)null);
            db.Setup(r => r.GetTrainingData(ID)).Returns(training.Object);

            calendar.Session["Email"] = EMAIL;
        }
    }
}
