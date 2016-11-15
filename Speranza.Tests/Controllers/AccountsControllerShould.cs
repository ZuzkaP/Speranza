﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Speranza.Controllers;
using Speranza.Models;
using Moq;
using Speranza.Database;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerShould
    {
        private AccountsController controller;
        private Mock<IDatabaseGateway> db;

        [TestMethod]
        public void ReturnCorrectView_When_RegisteringNewUser()
        {
            InitializeController();

            ViewResult result = controller.Register();

            Assert.AreEqual("Register", result.ViewName);

        }

        [TestMethod]
        public void NotRegisterAndReturnErrorMessage_When_EmailIsEmpty()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Email = String.Empty;

            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel) result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.EmailIsEmpty & modelFromServer.Messages);   
        }

        [TestMethod]
        public void NotRegisterAndReturnErrorMessage_When_PasswordIsEmpty()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = String.Empty;
            model.Email = "test@test.com";

            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.PasswordIsEmpty & modelFromServer.Messages);

        }

        [TestMethod]
        public void NotRegisterAndReturnErrorMessage_When_PasswordAndConfirmPasswordAreNotTheSame()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "1234Zuzka";
            model.Email = "test@test.com";
            model.ConfirmPassword = "1235Zuzka";

            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.ConfirmPassIncorrect & modelFromServer.Messages);
        }

        [TestMethod]
        public void RegisterNewUser_When_AllDataAreEntered()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "1234Zuzka";
            model.Email = "test@test.com";
            model.Name = "Zuzana";
            model.Surname = "Papalova";
            model.PhoneNumber = "0616554984899";
            model.ConfirmPassword = "1234Zuzka";

            ViewResult result = controller.Register(model);
            db.Verify(r => r.RegisterNewUser(model), Times.Once);
        }

        [TestMethod]
        public void NotRegisterNewUserAndReturnErrorMessage_When_UserAAlreadyExists()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "1234Zuzka";
            model.Email = "test@test.com";
            model.ConfirmPassword = "1234Zuzka";

            db.Setup(r => r.UserExists(model.Email)).Returns(true);

            ViewResult result = controller.Register(model);
            
            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.UserAlreadyExists & modelFromServer.Messages);
        }

        [TestMethod]
        public void NotRegisterNewUserAndReturnErrorMessage_When_EmailFormatIsIncorrect()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "1234Zuzka";
            model.Email = "test";
            model.ConfirmPassword = "1234Zuzka";
            
            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.EmailFormatIsIncorrect & modelFromServer.Messages);
            
        }

        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            controller = new AccountsController(db.Object);
        }
    }
}
