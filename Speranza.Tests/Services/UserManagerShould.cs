using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services.Interfaces;
using Speranza.Controllers;
using System.Web.SessionState;
using Speranza.Services;
using Speranza.Database.Data.Interfaces;
using Speranza.Tests.Controllers;
using Speranza.Models.Interfaces;
using System.Collections.Generic;
using Moq;
using Speranza.Database;
using Speranza.Common.Data;
using Speranza.Models;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class UserManagerShould
    {
        private SessionStateItemCollection collection;
        private UserManager manager;
        private FakeControllerContext context;
        private Mock<IDatabaseGateway> db;
        private Mock<IModelFactory> factory;
        private Mock<IUserForAdminModel> userModel1;
        private Mock<IUserForAdminModel> userModel2;
        private const string EMAIL = "test";
        private readonly int INCRESENUMBEROFSIGNUPS = 10;
        private readonly int DECREASENUMBEROFSIGNUPS = -10;
        private Mock<IDateTimeService> datetimeService;
        private Mock<ITrainingModel> training2Model;
        private Mock<ITrainingModel> training3Model;
        private const string NEWPASSWORDHASH = "hash";
        private Mock<IUserForTrainingDetailModel> user1Model;
        private Mock<IUserForTrainingDetailModel> user2Model;
        private Mock<IUserForTrainingDetailModel> user3Model;
        private const string SURNAME_FIRST = "surnameA";
        private const string SURNAME_THIRD = "surnameC";
        private const string SURNAME_SECOND= "surnameB";
        private const string PASSWORD_INCORRECT_HASH = "incorrectPassword";
        private const string PASSWORD_CORRECT_HASH = "Password";
        const UserCategories CATEGORY = UserCategories.Gold;
        const bool IS_ADMIN = true;
        private Mock<IHasher> hasher;
        private RegisterModel model;
        private Mock<IUserProfileModel> newUserProfileModel;
        private string NAME;
        private string PHONE_NUMBER;

        [TestMethod]
        public void ReturnFalse_When_SessionIsEmpty()
        {
            InitializeUserManager();
           Assert.IsFalse(manager.IsUserLoggedIn(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailInSessionDoesNotExist()
        {
            InitializeUserManager();
            collection["notEmail"] = "test";
            Assert.IsFalse(manager.IsUserLoggedIn(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailIsEmpty()
        {
            InitializeUserManager();
            collection["Email"] = "";
            Assert.IsFalse(manager.IsUserLoggedIn(context.HttpContext.Session));
            
        }

        [TestMethod]
        public void ReturnTrue_When_EmailSessionDoesExist()
        {
            InitializeUserManager();
            collection["Email"] = "test";
            Assert.IsTrue(manager.IsUserLoggedIn(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_AdminSessionIsNull()
        {
            InitializeUserManager();
            collection["IsAdmin"] = null;
            Assert.IsFalse(manager.IsUserAdmin(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_AdminSessionIsFalse()
        {
            InitializeUserManager();
            collection["IsAdmin"] = false;
            Assert.IsFalse(manager.IsUserAdmin(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnTrue_When_AdminSessionIsTrue()
        {
            InitializeUserManager();
            collection["IsAdmin"] = true;
            Assert.IsTrue(manager.IsUserAdmin(context.HttpContext.Session));
        }
        [TestMethod]
        public void ReturnStandardCategory_When_CategoryIsNotInSession()
        {
            InitializeUserManager();
            Assert.AreEqual(UserCategories.Standard,manager.GetUserCategory(context.HttpContext.Session));

        }

        [TestMethod]
        public void ReturnTheRightCategory_When_CategoryIsInSession()
        {
            InitializeUserManager();
            collection["Category"] = UserCategories.Gold;
            Assert.AreEqual(UserCategories.Gold, manager.GetUserCategory(context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnEmptyUsersList_When_NoUserIsInDb()
        {
            InitializeUserManager();
            PrepareDBWithNoUser();

            IList<IUserForAdminModel> usersList =manager.GetAllUsersForAdmin();

            Assert.IsNotNull(usersList);
            Assert.AreEqual(0, usersList.Count);

        }

        [TestMethod]
        public void ReturnUsersList_When_UsersExistInDb()
        {
            InitializeUserManager();
            PrepareDBWithTwoUsers();

            IList<IUserForAdminModel> usersList = manager.GetAllUsersForAdmin();

            Assert.IsNotNull(usersList);
            Assert.AreEqual(2, usersList.Count);
            Assert.AreEqual(userModel1.Object, usersList[0]);
            Assert.AreEqual(userModel2.Object, usersList[1]);

        }

        [TestMethod]
        public void ChangeCountOfFreeSignUps_When_AdminChangesTheValue()
        {
            InitializeUserManager();
            db.Setup(r => r.UpdateCountOfFreeSignUps(EMAIL, INCRESENUMBEROFSIGNUPS)).Returns(25);
           
            int updatedCount = manager.UpdateCountOfFreeSignUps(EMAIL, INCRESENUMBEROFSIGNUPS);

            db.Verify(r => r.UpdateCountOfFreeSignUps(EMAIL,INCRESENUMBEROFSIGNUPS), Times.Once);
            Assert.AreEqual(25, updatedCount);
        }

        [TestMethod]
        public void RegisterNewUser()
        {
            InitializeUserManager();
            PrepareDataFromRegisterModelToDB();

            manager.RegisterNewUser(model);

            db.Verify(r => r.RegisterNewUser(EMAIL,NAME,PASSWORD_CORRECT_HASH,PHONE_NUMBER,SURNAME_FIRST), Times.Once);
        }

        [TestMethod]
        public void UpdateUserData()
        {
            InitializeUserManager();
            PrepareDataFromRUserProfileModelToDB();

            manager.UpdateUserData(newUserProfileModel.Object);

            db.Verify(r => r.UpdateUserData(EMAIL, NAME, SURNAME_FIRST, PHONE_NUMBER), Times.Once);
        }

        private void PrepareDataFromRUserProfileModelToDB()
        {
            newUserProfileModel = new Mock<IUserProfileModel>();
            newUserProfileModel.SetupGet(r => r.Name).Returns(NAME);
            newUserProfileModel.SetupGet(r => r.Surname).Returns(SURNAME_FIRST);
            newUserProfileModel.SetupGet(r => r.Name).Returns(PHONE_NUMBER);
            newUserProfileModel.SetupGet(r => r.Email).Returns(EMAIL);
        }

        private void PrepareDataFromRegisterModelToDB()
        {
            model = new RegisterModel();
            model.Name = NAME;
            model.Surname = SURNAME_FIRST;
            model.PhoneNumber = PHONE_NUMBER;
            model.Email = EMAIL;
            model.Password = PASSWORD_CORRECT_HASH;
        }   

        private void PrepareDBWithTwoUsers()
        {
            var user1 = new Mock<IUser>();
            var user2 = new Mock<IUser>();
            var usersList = new List<IUser>() { user1.Object, user2.Object};
            db.Setup(r => r.GetAllUsers()).Returns(usersList);

            userModel1 = new Mock<IUserForAdminModel>();
            userModel2 = new Mock<IUserForAdminModel>();

            factory.Setup(r => r.CreateUserForAdminModel(user1.Object)).Returns(userModel1.Object);
            factory.Setup(r => r.CreateUserForAdminModel(user2.Object)).Returns(userModel2.Object);
        }


        [TestMethod]
        public void SetAdminRoleToUser()
        {
            InitializeUserManager();

            manager.SetUserRoleToAdmin(EMAIL, true);

            db.Verify(r => r.SetAdminRole(EMAIL, true), Times.Once);
        }

        [TestMethod]
        public void SetCategoryToUserByAdmin()
        {
            InitializeUserManager();

            manager.SetUserCategory(EMAIL, UserCategories.Gold);

            db.Verify(r => r.SetUserCategory(EMAIL, UserCategories.Gold), Times.Once);
        }

        [TestMethod]
        public void GetAllUsersForTrainingDetails()
        {
            InitializeUserManager();
            PrepareDBAndFactoryWithTwoUsers();

            var users = manager.GetAllUsersForTrainingDetails();

            Assert.AreEqual(3, users.Count);
            Assert.AreEqual(user2Model.Object, users[0]);
            Assert.AreEqual(user1Model.Object, users[1]);
            Assert.AreEqual(user3Model.Object, users[2]);
        }

        [TestMethod]
        public void ReturnTrue_When_UserExists()
        {
            InitializeUserManager();
            db.Setup(r => r.GetUserData(EMAIL)).Returns(new Mock<IUser>().Object);

            bool result = manager.UserExists(EMAIL);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ReturnFalse_When_UserDoesNotExist()
        {
            InitializeUserManager();
            db.Setup(r => r.GetUserData(EMAIL)).Returns((IUser)null);

            bool result = manager.UserExists(EMAIL);

            Assert.IsFalse(result);
        }


        [TestMethod]
        public void GetOnlyFutureTrainingsFromDB()
        {
            InitializeUserManager();
            PrepareDBAndFactoryWithThreeTrainings();
            
            IList<ITrainingModel> trainings = manager.GetFutureTrainingsForUser(EMAIL);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training2Model.Object, trainings[0]);
            Assert.AreEqual(training3Model.Object, trainings[1]);
        }

        [TestMethod]
        public void ChangePassword()
        {
            InitializeUserManager();

            manager.ChangePassword(EMAIL, NEWPASSWORDHASH);

            db.Verify(r => r.ChangePassword(EMAIL, NEWPASSWORDHASH));
        }

        [TestMethod]
        public void ReturnAddedUserData()
        {
            InitializeUserManager();
            PrepareDBAndFactoryWithOneUserForTRainingDetailModel();

            IUserForTrainingDetailModel model = manager.GetAddedUserData(EMAIL);

            db.Verify(r => r.GetUserData(EMAIL));
            Assert.AreEqual(user1Model.Object, model);

        }
        [TestMethod]
        public void ReturnNull_When_EmailDoesNotExist()
        {
            InitializeUserManager();
            db.Setup(r => r.LoadUser(EMAIL)).Returns((IUser)null);

            ILoginResult result = manager.Login(EMAIL, PASSWORD_CORRECT_HASH);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReturnNull_When_PassIsIncorrect()
        {
            InitializeUserManager();
            PrepareUserLoginDataInDB();

            ILoginResult result = manager.Login(EMAIL,PASSWORD_INCORRECT_HASH);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ReturnCorrectLoginResult_When_EmailAndPassAreCorrect()
        {
            InitializeUserManager();
            PrepareUserLoginDataInDB();
           
            ILoginResult result = manager.Login(EMAIL,PASSWORD_CORRECT_HASH);

            Assert.IsNotNull(result);
            Assert.AreEqual(EMAIL, result.Email);
            Assert.AreEqual(CATEGORY, result.Category);
            Assert.AreEqual(IS_ADMIN, result.IsAdmin);
        }
        private void PrepareUserLoginDataInDB()
        {
            var user = new Mock<IUser>();
            user.SetupGet(r => r.PasswordHash).Returns(PASSWORD_CORRECT_HASH);
            user.SetupGet(r => r.Category).Returns(CATEGORY);
            user.SetupGet(r => r.IsAdmin).Returns(IS_ADMIN);
            db.Setup(r => r.LoadUser(EMAIL)).Returns(user.Object);
        }

        private void PrepareDBAndFactoryWithThreeTrainings()
        {
            Mock<ITraining> training1 = new Mock<ITraining>();
            Mock<ITraining> training2 = new Mock<ITraining>();
            Mock<ITraining> training3 = new Mock<ITraining>();
            training2Model = new Mock<ITrainingModel>();
            training3Model = new Mock<ITrainingModel>();
            training1.SetupGet(r => r.Time).Returns(new DateTime(2016, 12, 31));
            training2.SetupGet(r => r.Time).Returns(new DateTime(2017, 01, 20));
            training3.SetupGet(r => r.Time).Returns(new DateTime(2017, 01, 15));
            IList<ITraining> userTrainings = new List<ITraining>() { training1.Object,training2.Object,training3.Object};
            db.Setup(r => r.GetTrainingsForUser(EMAIL)).Returns(userTrainings);
            factory.Setup(r => r.CreateTrainingModel(training2.Object)).Returns(training2Model.Object);
            factory.Setup(r => r.CreateTrainingModel(training3.Object)).Returns(training3Model.Object);
        }

        private void PrepareDBWithNoUser()
        {
            db.Setup(r => r.GetAllUsers()).Returns(new List<IUser>());
        }

        private void PrepareDBAndFactoryWithTwoUsers()
        {
            var user1 = new Mock<IUser>();
            var user2 = new Mock<IUser>();
            var user3 = new Mock<IUser>();
            var users = new List<IUser>() { user1.Object, user2.Object , user3.Object};
            db.Setup(r => r.GetAllUsers()).Returns(users);

            user1Model = new Mock<IUserForTrainingDetailModel>();
            user2Model = new Mock<IUserForTrainingDetailModel>();
            user3Model = new Mock<IUserForTrainingDetailModel>();

            user1Model.SetupGet(r => r.Surname).Returns(SURNAME_SECOND);
            user2Model.SetupGet(r => r.Surname).Returns(SURNAME_FIRST);
            user3Model.SetupGet(r => r.Surname).Returns(SURNAME_THIRD);

            factory.Setup(r => r.CreateUsersForTrainingDetailModel(user1.Object)).Returns(user1Model.Object);
            factory.Setup(r => r.CreateUsersForTrainingDetailModel(user2.Object)).Returns(user2Model.Object);
            factory.Setup(r => r.CreateUsersForTrainingDetailModel(user3.Object)).Returns(user3Model.Object);
        }


        private void PrepareDBAndFactoryWithOneUserForTRainingDetailModel()
        {
            var user1 = new Mock<IUser>();
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user1.Object);
            user1Model = new Mock<IUserForTrainingDetailModel>();
            
            factory.Setup(r => r.CreateUsersForTrainingDetailModel(user1.Object)).Returns(user1Model.Object);
        }
        private void InitializeUserManager()
        {
            factory = new Mock<IModelFactory>();
            db = new Mock<IDatabaseGateway>();
            datetimeService = new Mock<IDateTimeService>();
            hasher = new Mock<IHasher>();
            manager = new UserManager(db.Object, factory.Object,datetimeService.Object,hasher.Object);
            collection = new SessionStateItemCollection();
            context = new FakeControllerContext(null, collection);
            datetimeService.Setup(r => r.GetCurrentDate()).Returns(new DateTime(2017, 01, 06));
           
        }
    }
}
