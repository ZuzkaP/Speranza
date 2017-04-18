using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using Moq;
using Speranza.Database;
using Speranza.Models;
using Speranza.Services;
using System.Web.Mvc;
using System.Web;
using System.Web.SessionState;
using Speranza.Database.Data.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Common.Data;
using Speranza.Models.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerLoginShould
    {
        private AccountsController controller;
        private Mock<IDatabaseGateway> db;
        private Mock<IHasher> hasher;
        private Mock<IModelFactory> factory;
        private Mock<IUserManager> userManager;
        private LoginModel model;
        private const string HASH = "hash";
        private const string PASSWORD = "pass";
        private const string EMAIL = "email";
        const UserCategories CATEGORY = UserCategories.Gold;
        private const string COOKIE_SERIES = "Series";
        private const string COOKIE_TOKEN = "Token";
        private Mock<ILoginResult> loginResult;
        private Mock<ICookieService> cookieService;
        private Mock<IUidService> uidService;

        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            hasher = new Mock<IHasher>();
            factory = new Mock<IModelFactory>();
            userManager = new Mock<IUserManager>();
            cookieService = new Mock<ICookieService>();
            uidService = new Mock<IUidService>();
            controller = new AccountsController(db.Object,hasher.Object,userManager.Object,null,null,factory.Object, cookieService.Object,uidService.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            HttpCookieCollection cookies = new HttpCookieCollection();

            controller.ControllerContext = new FakeControllerContext(controller, sessionItems, cookies);
        }

        [TestMethod]
        public void CorrectlyInitializeSessionAndRedirectToCalendar_When_AdminLoginWasSuccessfull()
        {
            InitializeController();
            PrepareDataForSuccessfullAdminLogin();

            ActionResult result = controller.Login(model);

            Assert.AreEqual(EMAIL,controller.Session["Email"]);
            Assert.AreEqual(CATEGORY,controller.Session["Category"]);
            Assert.IsTrue((bool)controller.Session["IsAdmin"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            cookieService.Verify(r => r.SetRememberMeCookie(It.IsAny<HttpCookieCollection>(),It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(r => r.SetRememberMe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);


        }

        [TestMethod]
        public void CorrectlyInitializeSessionAndRedirectToCalendar_When_NonAdminLoginWasSuccessfull()
        {
            InitializeController();
            PrepareDataForSuccessfulNonAdminLogin();

            ActionResult result = controller.Login(model);

            Assert.AreEqual(EMAIL, controller.Session["Email"]);
            Assert.AreEqual(CATEGORY, controller.Session["Category"]);
            Assert.IsFalse((bool)controller.Session["IsAdmin"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
            cookieService.Verify(r => r.SetRememberMeCookie(It.IsAny<HttpCookieCollection>(),It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(r => r.SetRememberMe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);


        }

        private void PrepareDataForSuccessfulNonAdminLogin()
        {
            PrepareDataForSuccessfullAdminLogin();
            loginResult.SetupGet(r => r.IsAdmin).Returns(false);
        }

        private void PrepareDataForSuccessfullAdminLogin()
        {
            model = new LoginModel();
            model.Email = EMAIL;
            model.Password = PASSWORD;
            model.RememberMe = false;
            hasher.Setup(r => r.HashPassword(model.Password)).Returns(HASH);

            loginResult = new Mock<ILoginResult>();
            loginResult.SetupGet(r => r.Category).Returns(CATEGORY);
            loginResult.SetupGet(r => r.Email).Returns(EMAIL);
            loginResult.SetupGet(r => r.IsAdmin).Returns(true);
            userManager.Setup(r => r.Login(EMAIL, HASH)).Returns(loginResult.Object);
        }

        [TestMethod]
        public void NotInitializeSessionAndStayInLoginScreen_When_LoginWasNotSuccessfull()
        {
            InitializeController();
            PrepareNotSuccessfulLogin();

            ViewResult result = (ViewResult)controller.Login(model);

            Assert.IsNull(controller.Session["Email"]);
            Assert.IsNull(controller.Session["Category"]);
            Assert.IsNull(controller.Session["IsAdmin"]);
            Assert.AreEqual("../Home/Index", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsFalse(modelFromServer.LoginSuccessful);
            cookieService.Verify(r => r.SetRememberMeCookie(It.IsAny<HttpCookieCollection>(),It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(r => r.SetRememberMe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }

        private void PrepareNotSuccessfulLogin()
        {
            model = new LoginModel();
            model.Email = EMAIL;
            model.Password = PASSWORD;
            hasher.Setup(r => r.HashPassword(model.Password)).Returns(HASH);
            userManager.Setup(r => r.Login(EMAIL, HASH)).Returns((ILoginResult)null);
        }

        [TestMethod]
        public void NotLogin_When_EmailIsEmpty()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = string.Empty;

            ViewResult result = (ViewResult) controller.Login(model);

            db.Verify(r => r.LoadUser(It.IsAny<string>()), Times.Never);
            Assert.AreEqual("../Home/Index", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsFalse(modelFromServer.LoginSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));
            cookieService.Verify(r => r.SetRememberMeCookie(It.IsAny<HttpCookieCollection>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(r => r.SetRememberMe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);



        }

        [TestMethod]
        public void NotLogin_When_PasswordIsEmpty()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            hasher.Setup(r => r.HashPassword(null)).Throws(new ArgumentNullException());
            model.Email = EMAIL;
            model.Password = null;

            ViewResult result = (ViewResult)controller.Login(model);

            db.Verify(r => r.LoadUser(It.IsAny<string>()), Times.Never);
            Assert.AreEqual("../Home/Index", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsFalse(modelFromServer.LoginSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));
            cookieService.Verify(r => r.SetRememberMeCookie(It.IsAny<HttpCookieCollection>(),It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            userManager.Verify(r => r.SetRememberMe(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        }
        
        [TestMethod]
        public void Logout_When_Requested()
        {
            InitializeController();
            controller.Session["Email"] = EMAIL;
            ActionResult result = controller.Logout();

            Assert.AreEqual(null, controller.Session["Email"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
            userManager.Verify(r => r.CancelRememberMe(EMAIL), Times.Once);
        }

        [TestMethod]
        public void CorrectlyCreateCookiesWhenLoginWasSuccessfull_And_RememberMeWasChecked()
        {
            InitializeController();
            PrepareDataForSuccessfulNonAdminLogin();
            PrepareUidServiceForRememberMe();
            PrepareCheckedRememberMe();

            ActionResult result = controller.Login(model);
            
            cookieService.Verify(r => r.SetRememberMeCookie(controller.Response.Cookies,COOKIE_SERIES, COOKIE_TOKEN), Times.Once);
            userManager.Verify(r => r.SetRememberMe(EMAIL, COOKIE_SERIES, COOKIE_TOKEN), Times.Once);

        }

        private void PrepareUidServiceForRememberMe()
        {
            uidService.Setup(r => r.GenerateSeries()).Returns(COOKIE_SERIES);
            uidService.Setup(r => r.GenerateToken()).Returns(COOKIE_TOKEN);
        }

        private void PrepareCheckedRememberMe()
        {
            model.RememberMe = true;
        }
    }
    
}
