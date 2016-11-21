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
            controller = new AccountsController(db.Object,hasher.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();

            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
        }
        
        [TestMethod]
        public void NotLogin_When_EmailIsEmpty()
        {
            InitializeController();
            LoginModel model = new LoginModel();
            model.Email = string.Empty;

            ViewResult result = controller.Login(model);

            db.Verify(r => r.LoadUser(It.IsAny<LoginModel>()), Times.Never);
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
            
            db.Setup(r => r.LoadUser(model)).Returns((IUser)null);
            ViewResult result = controller.Login(model);
            
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
            db.Setup(r => r.LoadUser(model)).Returns(user.Object);
            ViewResult result = controller.Login(model);

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
            db.Setup(r => r.LoadUser(model)).Returns(user.Object);

            ViewResult result = controller.Login(model);

            Assert.AreEqual(model.Email, this.controller.Session["Email"]);
            Assert.AreEqual("Calendar", result.ViewName);

            LoginModel modelFromServer = (LoginModel)result.Model;
            Assert.IsTrue(modelFromServer.LoginSuccessful);
        }
    }
    
}