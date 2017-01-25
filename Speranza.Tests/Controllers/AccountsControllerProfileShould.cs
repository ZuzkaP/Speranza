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
        Mock<ITrainingModel> trainingModel;

        private readonly string NAME = "Zuzka";
        private readonly string SURNAME = "Papalova";
        private readonly string PHONENUMBER = "1234";
        private Mock<IDateTimeService> dateTimeService;
        private readonly DateTime date = new DateTime(2016, 12, 2, 10, 00, 00);

        public const string CATEGORY = "Silver";
        public const int NUMBEROFFREESIGNUPS = 5;
        public const int NUMBEROFPASTRAININGS = 42;
        private Mock<IModelFactory> factory;
        private Mock<IHasher> hasher;

        [TestMethod]
        public void ReturnToLogin_When_UserIsNotLoggedIn()
        {
            InitializeAccountController();
            userManager.Setup(r => r.IsUserLoggedIn(controller.Session)).Returns(false);
            ActionResult result = controller.UserProfile();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void LoadUSerDatafromDBAndSendToUI_When_UserIsLoggedIn()
        {
            InitializeAccountController();

            ViewResult result = (ViewResult)controller.UserProfile();

            UserProfileModel model = (UserProfileModel) result.Model;
            Assert.AreEqual(NAME, model.Name);
            Assert.AreEqual(SURNAME, model.Surname);
            Assert.AreEqual(PHONENUMBER, model.PhoneNumber);
            Assert.AreEqual(USER_EMAIL, model.Email);

        }

        [TestMethod]
        public void LoadCategory_And_NumberOfFreeSignUps_And_NumberOfPastTrainings()
        {
            InitializeAccountController();
            PrepareUserFromDB();

            ViewResult result = (ViewResult)controller.UserProfile();

            UserProfileModel model = (UserProfileModel)result.Model;

            Assert.AreEqual(CATEGORY, model.Category);
            Assert.AreEqual(NUMBEROFFREESIGNUPS, model.NumberOfFreeSignUps);
            Assert.AreEqual(NUMBEROFPASTRAININGS, model.NumberOfPastTrainings);
        }

        private void PrepareUserFromDB()
        {
            userData.SetupGet(u => u.Category).Returns(UserCategories.Silver);
            userData.SetupGet(u => u.NumberOfFreeSignUpsOnSeasonTicket).Returns(NUMBEROFFREESIGNUPS);
            userData.SetupGet(u => u.NumberOfPastTrainings).Returns(NUMBEROFPASTRAININGS);
        }

        [TestMethod]
        public void ReturnToLogin_When_SavingUserProfileAndUserIsNotLoggedIn()
        {
            InitializeAccountController();
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
            InitializeAccountController();
            UserProfileModel model = new UserProfileModel();
            ActionResult result = controller.SaveUserProfile(model);
            Assert.AreEqual("UserProfile", ((RedirectToRouteResult)result).RouteValues["action"]);
            db.Verify(r => r.UpdateUserData(It.Is<UserProfileModel>(k=>k==model && !string.IsNullOrEmpty(model.Email) && model.Email == USER_EMAIL)));
        }


        [TestMethod]
        public void ShowOnlyAccountInf_When_UserIsNotSignedUpForTraining()
        {
            InitializeAccountController();
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(new List<ITraining>());

            ViewResult result = (ViewResult)controller.UserProfile();
        
            UserProfileModel model = (UserProfileModel)result.Model;
            Assert.AreEqual(0, model.FutureTrainings.Count);
        }


        [TestMethod]
        public void ShowTrainingInfo_When_UserIsSignedUpToTraining()
        {
            InitializeAccountController();
            IList<ITraining> trainings = new List<ITraining>();
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(trainings);
            Mock<ITraining> training1 = new Mock<ITraining>();
            Mock<ITrainingModel> training1Model = new Mock<ITrainingModel>();
            factory.Setup(r => r.CreateTrainingModel(training1.Object)).Returns(training1Model.Object);
            trainings.Add(training1.Object);

            ViewResult result = (ViewResult)controller.UserProfile();

            UserProfileModel model = (UserProfileModel)result.Model;
            Assert.AreEqual(1, model.FutureTrainings.Count);
            Assert.AreEqual(training1Model.Object, model.FutureTrainings[0]);
        }

        [TestMethod]
        public void SendMessageToUI_When_SignOffFromTraining()
        {
            InitializeAccountController();

            controller.Session["Message"] = CalendarMessages.UserWasSignedOff;
            ActionResult result = controller.UserProfile();
            UserProfileModel model = (UserProfileModel)((ViewResult)result).Model;

            Assert.AreEqual(CalendarMessages.UserWasSignedOff, model.Message);

        }
        
        [TestMethod]
        public void SendTrainingDataToUI_When_SignOffFromTraining()
        {
            InitializeAccountController();
            Mock<ITrainingModel> trainingModel = new Mock<ITrainingModel>();
            controller.Session["Training"] = trainingModel.Object;

            ActionResult result = controller.UserProfile();

            UserProfileModel model = (UserProfileModel)((ViewResult)result).Model;
            Assert.AreEqual(trainingModel.Object, model.SignedUpOrSignedOffTraining);

        }

        [TestMethod]
        public void OrderTrainingsByDate()
        {
            InitializeAccountController();
            
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
            factory.Setup(r => r.CreateTrainingModel(trainingA.Object)).Returns(trainingModelA.Object);
            factory.Setup(r => r.CreateTrainingModel(trainingB.Object)).Returns(trainingModelB.Object);
            factory.Setup(r => r.CreateTrainingModel(trainingC.Object)).Returns(trainingModelC.Object);
            factory.Setup(r => r.CreateTrainingModel(trainingD.Object)).Returns(trainingModelD.Object);

            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(new DateTime(2016, 12, 17));

            ActionResult result = controller.UserProfile();

            UserProfileModel model = (UserProfileModel)((ViewResult)result).Model;
            Assert.AreEqual(trainingModelA.Object, model.FutureTrainings[0]);
            Assert.AreEqual(trainingModelD.Object, model.FutureTrainings[1]);
            Assert.AreEqual(trainingModelC.Object, model.PastTrainings[0]);
            Assert.AreEqual(trainingModelB.Object, model.PastTrainings[1]);
            Assert.AreEqual(2, model.FutureTrainings.Count);
            Assert.AreEqual(2, model.PastTrainings.Count);
        }

        [TestMethod]
        public void NotAllowToSignOffFromCloseFutureTraining()
        {
            InitializeAccountController();
            PrepareTrainingInCloseFuture();

            controller.UserProfile();

            trainingModel.VerifySet(r => r.IsAllowedToSignOff = false);
        }
        
        [TestMethod]
        public void AllowToSignOffFromMoreDistantFutureTraining()
        {
            InitializeAccountController();
            PrepareTrainingInDistantFuture();

            controller.UserProfile();

            trainingModel.VerifySet(r => r.IsAllowedToSignOff = true);
        }

        [TestMethod]
        public void NotChangePass_When_PassIsNotTheSameAsHash()
        {
            InitializeAccountController();


        }


        private void PrepareTrainingInDistantFuture()
        {
            Mock<ITraining> trainingD = new Mock<ITraining>();
            IList<ITraining> trainings = new List<ITraining>() { trainingD.Object };
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(trainings);

            trainingModel = new Mock<ITrainingModel>();
            trainingModel.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 2, 18, 00, 00));
            factory.Setup(r => r.CreateTrainingModel(trainingD.Object)).Returns(trainingModel.Object);

            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(date);
        }

        private void PrepareTrainingInCloseFuture()
        {
            Mock<ITraining> trainingD = new Mock<ITraining>();
            IList<ITraining> trainings = new List<ITraining>() {trainingD.Object };
            db.Setup(r => r.GetTrainingsForUser(USER_EMAIL)).Returns(trainings);

            trainingModel = new Mock<ITrainingModel>();
            trainingModel.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 2, 12,00,00));
            factory.Setup(r => r.CreateTrainingModel(trainingD.Object)).Returns(trainingModel.Object);

            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(date);
        }
        
        private void InitializeAccountController()
        {
            db = new Mock<IDatabaseGateway>();
            userManager = new Mock<IUserManager>();
            trainingManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            factory = new Mock<IModelFactory>();
            hasher = new Mock<IHasher>();
            controller = new AccountsController(db.Object,null,userManager.Object, trainingManager.Object,dateTimeService.Object,factory.Object);
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
