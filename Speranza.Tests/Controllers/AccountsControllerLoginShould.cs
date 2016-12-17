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

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerLoginShould
    {
        private AccountsController controller;
        private Mock<IDatabaseGateway> db;
        private Mock<IUser> user;
        private Mock<IHasher> hasher;

        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            hasher = new Mock<IHasher>();
            controller = new AccountsController(db.Object,hasher.Object,null,null,null);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();

            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
        }
        
        [TestMethod]
        public void NotLogin_When_EmailIsEmpty()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = string.Empty;

            ViewResult result = (ViewResult) controller.Login(model);

            db.Verify(r => r.LoadUser(It.IsAny<string>()), Times.Never);
            Assert.AreEqual("Index", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsFalse(modelFromServer.LoginSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));

        }
        
        [TestMethod]
        public void NotLogin_When_EmailDoesNotExist()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = "test@test.com";
            
            db.Setup(r => r.LoadUser(model.Email)).Returns((IUser)null);
            ViewResult result = (ViewResult) controller.Login(model);
            
            Assert.AreEqual("Index", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsFalse(modelFromServer.LoginSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));
        }

        [TestMethod]
        public void NotLogin_When_PassIsIncorrect()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = "test@test.com";
            model.Password = "incorrectPassword";
            hasher.Setup(r => r.HashPassword(model.Password)).Returns("incorrectHash");
            user = new Mock<IUser>();
            user.SetupGet(r => r.PasswordHash).Returns("hash");
            db.Setup(r => r.LoadUser(model.Email)).Returns(user.Object);
            ViewResult result = (ViewResult) controller.Login(model);

            Assert.AreEqual("Index", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsFalse(modelFromServer.LoginSuccessful);
            Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));
        }

        [TestMethod]
        public void Login_When_EmailAndPassAreCorrect()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = "test@test.com";
            model.Password = "Password";
            hasher.Setup(r => r.HashPassword(model.Password)).Returns("hash");
            user = new Mock<IUser>();
            user.SetupGet(r => r.PasswordHash).Returns("hash");
            db.Setup(r => r.LoadUser(model.Email)).Returns(user.Object);

            ActionResult result = controller.Login(model);

            Assert.AreEqual(model.Email, this.controller.Session["Email"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
           


        }

        [TestMethod]
        public void Logout_When_Requested()
        {
            InitializeController();
            controller.Session["Email"] = "test";
            ActionResult result = controller.Logout();

            Assert.AreEqual(null, controller.Session["Email"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }
    }
    
}
