using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using Moq;
using Speranza.Database;
using Speranza.Models;
using System.Web.Mvc;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerLoginShould
    {
        private AccountsController controller;
        private Mock<IDatabaseGateway> db;

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

        }

        private void InitializeController()
        {
            db = new Mock<IDatabaseGateway>();
            controller = new AccountsController(db.Object);
        }
    }
}
