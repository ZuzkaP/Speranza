using System;
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

            db.Verify(r => r.RegisterNewUser(),Times.Never);
            Assert.AreEqual("Register", result.ViewName);

            RegisterModel modelFromServer = (RegisterModel) result.Model;
            Assert.AreNotEqual(RegisterModelMessages.NoMessage, RegisterModelMessages.EmailIsEmpty & modelFromServer.Messages);   
        }


        private void InitializeController()
        {
            
            db = new Mock<IDatabaseGateway>();
            controller = new AccountsController(db.Object);
        }
    }
}
