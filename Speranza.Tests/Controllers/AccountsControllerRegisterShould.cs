using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Speranza.Controllers;
using Speranza.Models;
using Moq;
using Speranza.Database;
using Speranza.Services;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerRegisterShould
    {
        private AccountsController controller;
        private Mock<IDatabaseGateway> db;
        private Mock<IHasher> hasher;

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

            string hashedPassword = "hashPassword";
            hasher.Setup(r => r.HashPassword(model.Password)).Returns(hashedPassword);
            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(model), Times.Once);
            Assert.AreEqual(hashedPassword, model.Password);
            Assert.IsNull(model.ConfirmPassword);
            Assert.AreEqual("../Home/Index", result.ViewName);
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
        
        [TestMethod]
        public void NotRegisterNewUserAndReturnErrorMessage_When_PasswordIsTooShort()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "1Zuz";
            model.Email = "test@test.com";
            model.ConfirmPassword = "1Zuz";

            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.PasswordIsTooShort & modelFromServer.Messages);
        }


        [TestMethod]
        public void NotRegisterNewUserAndReturnErrorMessage_When_PasswordHasNoNumber()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "Zuzana";
            model.Email = "test@test.com";
            model.ConfirmPassword = "Zuzana";

            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.PasswordHasNoNumber & modelFromServer.Messages);
        }


        [TestMethod]
        public void NotRegisterNewUserAndReturnErrorMessage_When_PasswordHasNoLetters()
        {
            InitializeController();
            RegisterModel model = new RegisterModel();
            model.Password = "123456";
            model.Email = "test@test.com";
            model.ConfirmPassword = "123456";

            ViewResult result = controller.Register(model);

            db.Verify(r => r.RegisterNewUser(It.IsAny<RegisterModel>()), Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel)result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.PasswordHasNoLetter & modelFromServer.Messages);
        }


        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            hasher = new Mock<IHasher>();
            controller = new AccountsController(db.Object,hasher.Object,null,null,null,null);
        }
    }
}
