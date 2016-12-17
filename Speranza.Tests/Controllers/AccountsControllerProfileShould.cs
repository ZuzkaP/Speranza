using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.SessionState;
using Speranza.Controllers;
using Speranza.Services.Interfaces;
using Moq;
using System.Web.Mvc;
using Speranza.Database;
using Speranza.Database.Data.Interfaces;
using Moq.Language.Flow;
using Speranza.Models;
using System.Collections.Generic;
using Speranza.Models.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerProfileShould
    {
        private const string USER_EMAIL = "test";
        private AccountsController controller;
        private Mock<IUserManager> userManager;
        private Mock<ITrainingsManager> trainingManager;
        private Mock<IDatabaseGateway> db;
        private Mock<IUser> userData;

        private readonly string NAME = "Zuzka";
        private readonly string SURNAME = "Papalova";
        private readonly string PHONENUMBER = "1234";
        private Mock<IDateTimeService> dateTimeService;

        [TestMethod]
        public void ReturnToLogin_When_UserIsNotLoggedIn()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(false);
            ActionResult result = controller.UserProfile();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void LoadUSerDatafromDBAndSendToUI_When_UserIsLoggedIn()
        {
            InitializeController();
            ViewResult result = (ViewResult)controller.UserProfile();
            UserProfileModel model = (UserProfileModel) result.Model;
            Assert.AreEqual(NAME, model.Name);
            Assert.AreEqual(SURNAME, model.Surname);
            Assert.AreEqual(PHONENUMBER, model.PhoneNumber);
            Assert.AreEqual(USER_EMAIL, model.Email);

        }

        [TestMethod]
        public void ReturnToLogin_When_SavingUserProfileAndUserIsNotLoggedIn()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(false);
            ActionResult result = controller.SaveUserProfile(null);

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
            db.Verify(r => r.UpdateUserData(It.IsAny<UserProfileModel>()), Times.Never);

        }

        [TestMethod]
        public void SaveChanges_When_UserDataWereChanged()
        {
            InitializeController();
            UserProfileModel model = new UserProfileModel();
            ActionResult result = controller.SaveUserProfile(model);
            Assert.AreEqual("UserProfile", ((RedirectToRouteResult)result).RouteValues["action"]);
            db.Verify(r => r.UpdateUserData(It.Is<UserProfileModel>(k=>k==model && !string.IsNullOrEmpty(model.Email) && model.Email == USER_EMAIL)));
        }


        [TestMethod]
        public void ShowOnlyAccountInf_When_UserIsNotSignedUpForTraining()
        {
            InitializeController();
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(new List<ITraining>());

            ViewResult result = (ViewResult)controller.UserProfile();
        
            UserProfileModel model = (UserProfileModel)result.Model;
            Assert.AreEqual(0, model.Trainings.Count);
        }


        [TestMethod]
        public void ShowTrainingInfo_When_UserIsSignedUpToTraining()
        {
            InitializeController();
            IList<ITraining> trainings = new List<ITraining>();
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(trainings);
            Mock<ITraining> training1 = new Mock<ITraining>();
            Mock<ITrainingModel> training1Model = new Mock<ITrainingModel>();
            trainingManager.Setup(r => r.CreateModel(training1.Object)).Returns(training1Model.Object);
            trainings.Add(training1.Object);

            ViewResult result = (ViewResult)controller.UserProfile();

            UserProfileModel model = (UserProfileModel)result.Model;
            Assert.AreEqual(1, model.Trainings.Count);
            Assert.AreEqual(training1Model.Object, model.Trainings[0]);
        }

        [TestMethod]
        public void SendMessageToUI_When_SignOffFromTraining()
        {
            InitializeController();

            controller.Session["Message"] = CalendarMessages.UserWasSignedOff;
            ActionResult result = controller.UserProfile();
            UserProfileModel model = (UserProfileModel)((ViewResult)result).Model;

            Assert.AreEqual(CalendarMessages.UserWasSignedOff, model.Message);

        }
        
        [TestMethod]
        public void SendTrainingDataToUI_When_SignOffFromTraining()
        {
            InitializeController();
            Mock<ITrainingModel> trainingModel = new Mock<ITrainingModel>();
            controller.Session["Training"] = trainingModel.Object;

            ActionResult result = controller.UserProfile();

            UserProfileModel model = (UserProfileModel)((ViewResult)result).Model;
            Assert.AreEqual(trainingModel.Object, model.SignedUpOrSignedOffTraining);

        }

        [TestMethod]
        public void OrderTrainingsByDate()
        {
            InitializeController();
            
            Mock<ITraining> trainingA = new Mock<ITraining>();
            Mock<ITraining> trainingB = new Mock<ITraining>();
            Mock<ITraining> trainingC = new Mock<ITraining>();
            Mock<ITraining> trainingD = new Mock<ITraining>();
            IList<ITraining> trainings = new List<ITraining>() { trainingA.Object,trainingB.Object,trainingC.Object,trainingD.Object};
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(trainings);
            Mock<ITrainingModel> trainingModelA = new Mock<ITrainingModel>();
            Mock<ITrainingModel> trainingModelB = new Mock<ITrainingModel>();
            Mock<ITrainingModel> trainingModelC = new Mock<ITrainingModel>();
            Mock<ITrainingModel> trainingModelD = new Mock<ITrainingModel>();
            trainingModelA.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 20));
            trainingModelB.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 5));
            trainingModelC.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 12));
            trainingModelD.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 22));
            trainingManager.Setup(r => r.CreateModel(trainingA.Object)).Returns(trainingModelA.Object);
            trainingManager.Setup(r => r.CreateModel(trainingB.Object)).Returns(trainingModelB.Object);
            trainingManager.Setup(r => r.CreateModel(trainingC.Object)).Returns(trainingModelC.Object);
            trainingManager.Setup(r => r.CreateModel(trainingD.Object)).Returns(trainingModelD.Object);

            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(new DateTime(2016, 12, 17));

            ActionResult result = controller.UserProfile();

            UserProfileModel model = (UserProfileModel)((ViewResult)result).Model;
            Assert.AreEqual(trainingModelA.Object, model.Trainings[0]);
            Assert.AreEqual(trainingModelD.Object, model.Trainings[1]);
            Assert.AreEqual(trainingModelC.Object, model.Trainings[2]);
            Assert.AreEqual(trainingModelB.Object, model.Trainings[3]);


        }


        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            controller = new AccountsController(db.Object,null,userManager.Object, trainingManager.Object,dateTimeService.Object);
            userData = new Mock<IUser>();
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(true);
            controller.Session["Email"] = USER_EMAIL;
            userData.SetupGet(u => u.Name).Returns(NAME);
            userData.SetupGet(u => u.Surname).Returns(SURNAME);
            userData.SetupGet(u => u.PhoneNumber).Returns(PHONENUMBER);
            userData.SetupGet(u => u.Email).Returns(USER_EMAIL);


            db.Setup(r => r.GetUserData(USER_EMAIL)).Returns(userData.Object);

        }
    }
}
