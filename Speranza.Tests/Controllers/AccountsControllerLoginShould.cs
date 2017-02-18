﻿using System;
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

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerLoginShould
    {
        private AccountsController controller;
        private Mock<IDatabaseGateway> db;
        private Mock<IUser> user;
        private Mock<IHasher> hasher;
        private Mock<IModelFactory> factory;
        private Mock<IUserManager> userManager;

        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            hasher = new Mock<IHasher>();
            factory = new Mock<IModelFactory>();
            userManager = new Mock<IUserManager>();
            controller = new AccountsController(db.Object,hasher.Object,null,null,null,factory.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();

            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
        }
        //[TestMethod]
        //public void CorrectlyInitializeSessionAndRedirectToCalendar_When_AdminLoginWasSuccessfull()
        //{
        //    InitializeController();
        //    PrepareDataForSuccessfullAdminLogin();// mock na usera ze je admin, kategoria, a z neho model

        //    controller.Login();

        //    //testujem nastavenie session , kategoria,isAdmin,login successfull redirect
        //}

        [TestMethod]
        public void CorrectlyInitializeSessionAndRedirectToCalendar_When_NonAdminLoginWasSuccessfull()
        {
           

            //testujem nastavenie session , kategoria,isAdmin,login successfull redirect

        }

        private void PrepareDataForSuccessfullAdminLogin()
        {
            userManager.Setup(r => r.IsUserAdmin(controller.Session)).Returns(true);
        }

        [TestMethod]
        public void NotInitializeSessionAndStayInLoginScreen_When_LoginWasNotSuccessfull()
        {


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

        }
        
        //[TestMethod]
        //public void NotLogin_When_EmailDoesNotExist()
        //{
        //    InitializeController();
        //    LoginModel model = new LoginModel();
        //    model.Email = "test@test.com";
            
        //    db.Setup(r => r.LoadUser(model.Email)).Returns((IUser)null);
        //    ViewResult result = (ViewResult) controller.Login(model);
            
        //    Assert.AreEqual("../Home/Index", result.ViewName);

        //    LoginModel modelFromServer = (LoginModel)result.Model;
        //    Assert.IsFalse(modelFromServer.LoginSuccessful);
        //    Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));
        //}

        //[TestMethod]
        //public void NotLogin_When_PassIsIncorrect()
        //{
        //    InitializeController();
        //    LoginModel model = new LoginModel();
        //    model.Email = "test@test.com";
        //    model.Password = "incorrectPassword";
        //    hasher.Setup(r => r.HashPassword(model.Password)).Returns("incorrectHash");
        //    user = new Mock<IUser>();
        //    user.SetupGet(r => r.PasswordHash).Returns("hash");
        //    db.Setup(r => r.LoadUser(model.Email)).Returns(user.Object);
        //    ViewResult result = (ViewResult) controller.Login(model);

        //    Assert.AreEqual("../Home/Index", result.ViewName);

        //    LoginModel modelFromServer = (LoginModel)result.Model;
        //    Assert.IsFalse(modelFromServer.LoginSuccessful);
        //    Assert.IsTrue(string.IsNullOrEmpty(this.controller.Session["Email"] as string));
        //}

        //[TestMethod]
        //public void Login_When_EmailAndPassAreCorrect()
        //{
        //    InitializeController();
        //    LoginModel model = new LoginModel();
        //    model.Email = "test@test.com";
        //    model.Password = "Password";
        //    hasher.Setup(r => r.HashPassword(model.Password)).Returns("hash");
        //    user = new Mock<IUser>();
        //    user.SetupGet(r => r.PasswordHash).Returns("hash");
        //    db.Setup(r => r.LoadUser(model.Email)).Returns(user.Object);

        //    ActionResult result = controller.Login(model);

        //    Assert.AreEqual(model.Email, this.controller.Session["Email"]);
        //    Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        //    Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
        //    Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
        //    Assert.IsNotNull(this.controller.Session["IsAdmin"]);
        //    Assert.IsFalse((bool)this.controller.Session["IsAdmin"]);
        //}
        [TestMethod]
        public void SetAdminFlag_When_UserIsAdmin()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = "test@test.com";
            model.Password = "Password";
            hasher.Setup(r => r.HashPassword(model.Password)).Returns("hash");
            user = new Mock<IUser>();
            user.SetupGet(r => r.PasswordHash).Returns("hash");
            user.SetupGet(r => r.IsAdmin).Returns(true);
            db.Setup(r => r.LoadUser(model.Email)).Returns(user.Object);

            ActionResult result = controller.Login(model);

            Assert.IsNotNull(this.controller.Session["IsAdmin"]);
            Assert.IsTrue((bool)this.controller.Session["IsAdmin"]);
        }

        [TestMethod]
        public void SetCategory_When_UserIsLoggedIn()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = "test@test.com";
            model.Password = "Password";
            hasher.Setup(r => r.HashPassword(model.Password)).Returns("hash");
            user = new Mock<IUser>();
            user.SetupGet(r => r.PasswordHash).Returns("hash");
            user.SetupGet(r => r.Category).Returns(UserCategories.Silver);
            db.Setup(r => r.LoadUser(model.Email)).Returns(user.Object);

            ActionResult result = controller.Login(model);

            Assert.IsNotNull(this.controller.Session["Category"]);
            Assert.AreEqual(UserCategories.Silver,this.controller.Session["Category"]);
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
