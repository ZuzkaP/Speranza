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
        private const string NAME = "name";
        private const string PHONE_NUMBER = "phoneNumber";
        private const int FREE_SIGN_UPS = 5;
        private const int NUMBER_OF_PAST_TRAININGS = 8;
        private const string TRAINING_ID = "TRAINING_ID";
        private const string EMAIL2 = "EMAIL2";
        private const string TRAINING2_ID = "TRAINING2_ID";
        private const string NEW_PASS = "NewPass";
        private const string SERIES = "series";
        private const string TOKEN = "token";
        private const string NOT_PARSABLE_COOKIE = "abc";
        private readonly DateTime DATE_TIME = new DateTime(2017, 1, 6, 10, 00, 00);
        private Mock<IUserProfileModel> userProfileModel;
        private Mock<IUserInTraining> userInTraining;
        private Mock<IEmailManager> emailManager;
        private IList<IUser> users;
        private IList<IUser> admins;
        private Mock<IUserInTraining> user2InTraining;
        private Mock<IUserInTraining> user1InTraining;
        private Mock<IUser> user1;
        private Mock<IUser> user2;
        private Mock<IUidService> uidService;
        private const string MESSAGE = "message";

        [TestMethod]
        public void ReturnFalse_When_SessionIsEmpty()
        {
            InitializeUserManager();
           Assert.IsFalse(manager.IsUserLoggedIn(null, context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailInSessionDoesNotExist()
        {
            InitializeUserManager();
            collection["notEmail"] = "test";
            Assert.IsFalse(manager.IsUserLoggedIn(null, context.HttpContext.Session));
        }

        [TestMethod]
        public void ReturnFalse_When_EmailIsEmpty()
        {
            InitializeUserManager();
            collection["Email"] = "";
            Assert.IsFalse(manager.IsUserLoggedIn(null, context.HttpContext.Session));
            
        }

        [TestMethod]
        public void ReturnTrue_When_EmailSessionDoesExist()
        {
            InitializeUserManager();
            collection["Email"] = "test";
            Assert.IsTrue(manager.IsUserLoggedIn(null, context.HttpContext.Session));
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
            emailManager.Verify(r => r.SendWelcome(EMAIL), Times.Once);
        }

        [TestMethod]
        public void UpdateUserData()
        {
            InitializeUserManager();
            PrepareDataFromRUserProfileModelToDB();

            manager.UpdateUserData(newUserProfileModel.Object);

            db.Verify(r => r.UpdateUserData(EMAIL, NAME, SURNAME_FIRST, PHONE_NUMBER), Times.Once);
        }

        [TestMethod]
        public void GetAllowedToSignUpFlag()
        {
            InitializeUserManager();
            db.Setup(r => r.GetAllowedToSignUpFlag(EMAIL)).Returns(true);

            bool result = manager.GetAllowedToSignUpFlag(EMAIL);

            Assert.IsTrue(result);
            db.Verify(r => r.GetAllowedToSignUpFlag(EMAIL), Times.Once);
        }
        private void PrepareDataFromRUserProfileModelToDB()
        {
            newUserProfileModel = new Mock<IUserProfileModel>();
            newUserProfileModel.SetupGet(r => r.Name).Returns(NAME);
            newUserProfileModel.SetupGet(r => r.Surname).Returns(SURNAME_FIRST);
            newUserProfileModel.SetupGet(r => r.PhoneNumber).Returns(PHONE_NUMBER);
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

        [TestMethod]
        public void GetUserProfileDataAndReturnModel()
        {
            InitializeUserManager();
            PrepareUserProfileDataInDB();

            IUserProfileModel model = manager.GetUserProfileModelWithDataFromDB(EMAIL);
            
            Assert.AreEqual(userProfileModel.Object, model);
        }

        [TestMethod]
        public void NotUpdateCategory_When_UserLessThan40Visits()
        {
            InitializeUserManager();
            PreparUserWith40Visits();

            var result =  manager.UpdateUserCategory(EMAIL,UserCategories.Standard);

            db.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
            Assert.AreEqual(UserCategories.Standard, result);
        }

        [TestMethod]
        public void NotUpdateCategory_When_IsSilverAndSettingToSilver()
        {
            InitializeUserManager();
            PreparUserWith41Visits();

            var result = manager.UpdateUserCategory(EMAIL,UserCategories.Silver);

            db.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
            Assert.AreEqual(UserCategories.Silver, result);
        }

        [TestMethod]
        public void NotUpdateCategory_When_IsGoldAndSettingToSilver()
        {
            InitializeUserManager();
            PreparUserWith41Visits();

            var result = manager.UpdateUserCategory(EMAIL, UserCategories.Gold);

            db.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
            Assert.AreEqual(UserCategories.Gold, result);
        }


        [TestMethod]
        public void NotUpdateCategory_When_IsSilverAndSettingToStandard()
        {
            InitializeUserManager();
            PreparUserWith40Visits();

            var result = manager.UpdateUserCategory(EMAIL, UserCategories.Silver);

            db.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
            Assert.AreEqual(UserCategories.Silver, result);
        }
        
        [TestMethod]
        public void NotUpdateCategory_When_IsGoldAndSettingToStandard()
        {
            InitializeUserManager();
            PreparUserWith40Visits();

            var result = manager.UpdateUserCategory(EMAIL, UserCategories.Gold);

            db.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
            Assert.AreEqual(UserCategories.Gold, result);
        }

        [TestMethod]
        public void NotUpdateCategory_When_IsGoldAndSettingToGold()
        {
            InitializeUserManager();
            PreparUserWith81Visits();

            var result = manager.UpdateUserCategory(EMAIL, UserCategories.Gold);

            db.Verify(r => r.SetUserCategory(It.IsAny<string>(), It.IsAny<UserCategories>()), Times.Never);
            Assert.AreEqual(UserCategories.Gold, result);
        }

        [TestMethod]
        public void UpdateCategoryFromStandardToSilver_When_NumberOfVisitsIsHigerThan40()
        {
            InitializeUserManager();
            PreparUserWith41Visits();

            var result = manager.UpdateUserCategory(EMAIL,UserCategories.Standard);

            db.Verify(r => r.SetUserCategory(EMAIL, UserCategories.Silver), Times.Once);
            db.Verify(r => r.SetUserCategory(EMAIL, UserCategories.Gold), Times.Never);
            Assert.AreEqual(UserCategories.Silver, result);
        }

        [TestMethod]
        public void UpdateCategoryFromSilverToGold_When_NumberOfVisitsIsHigerThan80()
        {
            InitializeUserManager();
            PreparUserWith81Visits();

            var result = manager.UpdateUserCategory(EMAIL,UserCategories.Silver);

            db.Verify(r => r.SetUserCategory(EMAIL, UserCategories.Gold), Times.Once);
            db.Verify(r => r.SetUserCategory(EMAIL, UserCategories.Silver), Times.Never);
            Assert.AreEqual(UserCategories.Gold, result);
        }

        [TestMethod]
        public void ReturnFalse_And_NotSendNewPass_When_EmailIsInvalid()
        {
            InitializeUserManager();
            PrepareInvalidEmail();

            bool result = manager.SendNewPass(EMAIL);

            emailManager.Verify(r => r.SendPassRecoveryEmail(It.IsAny<string>(), It.IsAny<string>()),Times.Never);
            db.Verify(r => r.ChangePassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ReturnTrue_And_SendNewPass_When_EmailIsValid()
        {
            InitializeUserManager();
            PrepareValidEmail();

            bool result = manager.SendNewPass(EMAIL);

            emailManager.Verify(r => r.SendPassRecoveryEmail(EMAIL,NEW_PASS), Times.Once);
            db.Verify(r => r.ChangePassword(EMAIL, PASSWORD_CORRECT_HASH), Times.Once);
            Assert.IsTrue(result);
        }


        [TestMethod]
        public void StoreSeriesToken_IntoDB()
        {
            InitializeUserManager();

             manager.SetRememberMe(EMAIL, SERIES, TOKEN);

            db.Verify(r => r.SetRememberMe(EMAIL, SERIES, TOKEN), Times.Once);
        }


        [TestMethod]
        public void NotLogin_When_CookieIsNotParsable()
        {
            InitializeUserManager();

            var result = manager.VerifyRememberMe(NOT_PARSABLE_COOKIE);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void NotLogin_When_CookieIsParsable_And_NotValid()
        {
            InitializeUserManager();
            PrepareNotValidCookieInDB();

            var result = manager.VerifyRememberMe(SERIES + "=" + TOKEN);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Login_When_CookieIsParsable_And_Valid()
        {
            InitializeUserManager();
            PrepareValidCookieInDB();

            var result = manager.VerifyRememberMe(SERIES + "=" + TOKEN);

            Assert.IsNotNull(result);
            Assert.AreEqual(EMAIL, result.Email);
            Assert.AreEqual(CATEGORY, result.Category);
            Assert.AreEqual(IS_ADMIN, result.IsAdmin);
        }


        [TestMethod]
        public void NotLogin_When_CookieIsNotParsable_And_UserLoginIsChecked()
        {
            InitializeUserManager();

            var result = manager.IsUserLoggedIn(NOT_PARSABLE_COOKIE, context.HttpContext.Session);

            Assert.IsFalse(result);
            Assert.IsNull(context.HttpContext.Session["Email"]);
            Assert.IsNull(context.HttpContext.Session["Category"]);
            Assert.IsNull(context.HttpContext.Session["IsAdmin"]);
        }

        [TestMethod]
        public void NotLogin_When_CookieIsParsable_And_NotValid_And_UserLoginIsChecked()
        {
            InitializeUserManager();
            PrepareNotValidCookieInDB();

            var result = manager.IsUserLoggedIn(SERIES + "=" + TOKEN, context.HttpContext.Session);

            Assert.IsFalse(result);
            Assert.IsNull(context.HttpContext.Session["Email"]);
            Assert.IsNull(context.HttpContext.Session["Category"]);
            Assert.IsNull(context.HttpContext.Session["IsAdmin"]);
        }

        [TestMethod]
        public void Login_When_CookieIsParsable_And_Valid_And_UserLoginIsChecked()
        {
            InitializeUserManager();
            PrepareValidCookieInDB();

            var result = manager.IsUserLoggedIn(SERIES + "=" + TOKEN, context.HttpContext.Session);

            Assert.IsTrue(result);
            Assert.AreEqual(EMAIL, context.HttpContext.Session["Email"]);
            Assert.AreEqual(CATEGORY, context.HttpContext.Session["Category"]);
            Assert.AreEqual(IS_ADMIN, context.HttpContext.Session["IsAdmin"]);
        }


        [TestMethod]
        public void CancelRememberMe()
        {
            InitializeUserManager();

            manager.CancelRememberMe(EMAIL);

            db.Verify(r => r.CancelRememberMe(EMAIL), Times.Once);
        }


        private void PrepareValidCookieInDB()
        {
            user1 = new Mock<IUser>();
            user1.SetupGet(r => r.Category).Returns(CATEGORY);
            user1.SetupGet(r => r.IsAdmin).Returns(IS_ADMIN);
            user1.SetupGet(r => r.Email).Returns(EMAIL);
            db.Setup(r => r.LoadUser(SERIES, TOKEN)).Returns(user1.Object);
        }

        private void PrepareNotValidCookieInDB()
        {
            db.Setup(r => r.LoadUser(SERIES, TOKEN)).Returns((IUser)null);
        }

        private void PrepareValidEmail()
        {
            user1 = new Mock<IUser>();
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user1.Object);
            uidService.Setup(r => r.CreatePassword()).Returns(NEW_PASS);
            hasher.Setup(r => r.HashPassword(NEW_PASS)).Returns(PASSWORD_CORRECT_HASH);
        }

        private void PrepareInvalidEmail()
        {
            db.Setup(r => r.GetUserData(EMAIL)).Returns((IUser)null);
        }

        private void PreparUserWith81Visits()
        {
            db.Setup(r => r.GetNumberOfVisits(EMAIL,DATE_TIME)).Returns(81);
        }

        private void PreparUserWith41Visits()
        {
            db.Setup(r => r.GetNumberOfVisits(EMAIL, DATE_TIME)).Returns(41);
        }

        private void PreparUserWith40Visits()
        {
            db.Setup(r => r.GetNumberOfVisits(EMAIL, DATE_TIME)).Returns(40);
        }

        private void PrepareUserProfileDataInDB()
        {
           var user1 = new Mock<IUser>();
            user1.SetupGet(r => r.Name).Returns(NAME);
            user1.SetupGet(r => r.Surname).Returns(SURNAME_FIRST);
            user1.SetupGet(r => r.PhoneNumber).Returns(PHONE_NUMBER);
            user1.SetupGet(r => r.NumberOfFreeSignUpsOnSeasonTicket).Returns(FREE_SIGN_UPS);
            user1.SetupGet(r => r.NumberOfPastTrainings).Returns(NUMBER_OF_PAST_TRAININGS);
            user1.SetupGet(r => r.Email).Returns(EMAIL);
            user1.SetupGet(r => r.Category).Returns(CATEGORY);
            user1.SetupGet(r => r.IsAdmin).Returns(false);
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user1.Object);

            userProfileModel = new Mock<IUserProfileModel>();

            factory.Setup(r => r.CreateUserForUserProfileModel(user1.Object)).Returns(userProfileModel.Object);
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

        [TestMethod]
        public void DoNothing_When_NoNonprocessedUserInTraining()
        {
            InitializeUserManager();
            PrepareNoNonProcessedUserInTraining();

            manager.UpdateSeasonTickets();

            db.Verify(r => r.UpdateCountOfFreeSignUps(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            db.Verify(r => r.SetAlreadyProcessedFlag(It.IsAny<IUserInTraining>()), Times.Never);
        }

        [TestMethod]
        public void NotUpdateSeasonTicketButSetProcessedFlagAndZeroEntranceFlag_When_UserInTrainingHasNoEntrancesOnTicket()
        {
            InitializeUserManager();
            PrepareUserInTRainingWithoutEntranceOnTicket();

            manager.UpdateSeasonTickets();

            db.Verify(r => r.UpdateCountOfFreeSignUps(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            db.Verify(r => r.SetAlreadyProcessedFlag(userInTraining.Object), Times.Once);
            db.Verify(r => r.SetZeroEntranceFlag(userInTraining.Object, true), Times.Once);

        }

        [TestMethod]
        public void UpdateSeasonTicketandSetProcessedFlagAndDoNotSetZeroEntranceFlag_When_UserInTrainingHasEntrancesOnTicket()
        {
            InitializeUserManager();
            PrepareUserInTRainingWithEntranceOnTicket();

            manager.UpdateSeasonTickets();

            db.Verify(r => r.UpdateCountOfFreeSignUps(EMAIL, -1), Times.Once);
            db.Verify(r => r.SetAlreadyProcessedFlag(userInTraining.Object), Times.Once);
            db.Verify(r => r.SetZeroEntranceFlag(userInTraining.Object, false), Times.Once);
        }

        [TestMethod]
        public void NotSendEmailToAdmins_When_NoUserWithZeroEntranceFlagWasInTraining()
        {
            InitializeUserManager();
            PrepareTrainingWithNoUserWithZeroEntranceFlag();

            manager.PromptToConfirmUserAttendance();

            emailManager.Verify(r => r.SendConfirmUserAttendance(It.IsAny<IList<IUser>>(), It.IsAny<IList<ITraining>>()), Times.Never);
            db.Verify(r => r.SetZeroEntranceFlag(It.IsAny<IUserInTraining>(),It.IsAny<bool>()), Times.Never);
        }

        private void PrepareTrainingWithNoUserWithZeroEntranceFlag()
        {
            db.Setup(r => r.GetAllUsersInTrainingWithZeroEntranceFlag()).Returns(new List<IUserInTraining>());
        }

        [TestMethod]
        public void SendEmailToAdmins_When_TwoUsersWithZeroEntranceFlagExistInOneTraining()
        {
            InitializeUserManager();
            PrepareTrainingWithTwoUsersWithZeroEntranceFlag();
            PrepareAdmins();

            manager.PromptToConfirmUserAttendance();

            emailManager.Verify(r => r.SendConfirmUserAttendance(admins,It.IsAny<IList<ITraining>>()), Times.Once);
            db.Verify(r => r.SetZeroEntranceFlag(user1InTraining.Object, false), Times.Once);
            db.Verify(r => r.SetZeroEntranceFlag(user2InTraining.Object, false), Times.Once);
        }
        
        [TestMethod]
        public void CleanUpTokensPeriodically()
        {
            InitializeUserManager();

            manager.CleanUpTokens();

            db.Verify(r => r.CleanUpTokens(), Times.Once);
        }

        [TestMethod]
        public void RemoveAllUserData_When_Requested()
        {
            InitializeUserManager();
            manager.RemoveAccountFromDB(EMAIL);

            db.Verify(r => r.SignOutUserFromAllTrainingsAfterDate(EMAIL,DATE_TIME), Times.Once);
            db.Verify(r => r.CancelRememberMe(EMAIL), Times.Once);
            db.Verify(r => r.RemoveAccountFromDB(EMAIL), Times.Once);
        }



        private void PrepareTwoTrainingWithTwoUsersWithZeroEntranceFlag()
        {
            var training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);

            var training2 = new Mock<ITraining>();
            training2.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING2_ID)).Returns(training2.Object);

            user1InTraining = new Mock<IUserInTraining>();
            user1InTraining.SetupGet(r => r.Email).Returns(EMAIL);
            user1InTraining.SetupGet(r => r.TrainingID).Returns(TRAINING_ID);
            user1 = new Mock<IUser>();
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user1.Object);

            user2InTraining = new Mock<IUserInTraining>();
            user2InTraining.SetupGet(r => r.Email).Returns(EMAIL2);
            user2InTraining.SetupGet(r => r.TrainingID).Returns(TRAINING2_ID);
            user2 = new Mock<IUser>();
            db.Setup(r => r.GetUserData(EMAIL2)).Returns(user2.Object);

            var listOfUsersInTraining = new List<IUserInTraining>() { user1InTraining.Object, user2InTraining.Object };
            db.Setup(r => r.GetAllUsersInTrainingWithZeroEntranceFlag()).Returns(listOfUsersInTraining);

        }

        private void PrepareAdmins()
        {
            admins = new List<IUser>();
            db.Setup(r => r.GetAdmins()).Returns(admins);
        }

        private void PrepareTrainingWithTwoUsersWithZeroEntranceFlag()
        {
            var training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);
            user1InTraining = new Mock<IUserInTraining>();
            user1InTraining.SetupGet(r => r.Email).Returns(EMAIL);
            user1InTraining.SetupGet(r => r.TrainingID).Returns(TRAINING_ID);
            user1 = new Mock<IUser>();
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user1.Object);
            user2InTraining = new Mock<IUserInTraining>();
            user2InTraining.SetupGet(r => r.Email).Returns(EMAIL2);
            user2InTraining.SetupGet(r => r.TrainingID).Returns(TRAINING_ID);
            user2 = new Mock<IUser>();
            db.Setup(r => r.GetUserData(EMAIL2)).Returns(user2.Object);
            var listOfUsersInTraining = new List<IUserInTraining>() { user1InTraining.Object, user2InTraining.Object};
            db.Setup(r => r.GetAllUsersInTrainingWithZeroEntranceFlag()).Returns(listOfUsersInTraining);
        }

        private void PrepareUserInTRainingWithEntranceOnTicket()
        {
            userInTraining = new Mock<IUserInTraining>();
            userInTraining.SetupGet(r => r.Email).Returns(EMAIL);
            db.Setup(r => r.GetNonProcessedUsersInTrainingBeforeDate(DATE_TIME)).Returns(new List<IUserInTraining>() { userInTraining.Object });
            var user = new Mock<IUser>();
            user.SetupGet(r => r.NumberOfFreeSignUpsOnSeasonTicket).Returns(5);
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user.Object);
        }

        private void PrepareUserInTRainingWithoutEntranceOnTicket()
        {
            userInTraining = new Mock<IUserInTraining>();
            userInTraining.SetupGet(r => r.Email).Returns(EMAIL);
            db.Setup(r => r.GetNonProcessedUsersInTrainingBeforeDate(DATE_TIME)).Returns(new List<IUserInTraining>() { userInTraining.Object });
            var user = new Mock<IUser>();
            user.SetupGet(r => r.NumberOfFreeSignUpsOnSeasonTicket).Returns(0);
            db.Setup(r => r.GetUserData(EMAIL)).Returns(user.Object);
        }

        private void PrepareNoNonProcessedUserInTraining()
        {
            db.Setup(r => r.GetNonProcessedUsersInTrainingBeforeDate(DATE_TIME)).Returns(new List<IUserInTraining>());
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
            emailManager = new Mock<IEmailManager>();
            uidService = new Mock<IUidService>();
            manager = new UserManager(db.Object, factory.Object,datetimeService.Object,hasher.Object, emailManager.Object, uidService.Object);
            collection = new SessionStateItemCollection();
            context = new FakeControllerContext(null, collection);
            datetimeService.Setup(r => r.GetCurrentDateTime()).Returns(DATE_TIME);
          
           
        }
    }
}
