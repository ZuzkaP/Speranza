using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using Speranza.Controllers;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerShould
    {
        private AccountsController controller;

        [TestMethod]
        public void ReturnCorrectView_When_RegisteringNewUser()
        {
            InitializeController();

            ViewResult result = controller.Register();

            Assert.AreEqual("Register", result.ViewName);

        }

        private void InitializeController()
        {
            controller = new AccountsController();
        }
    }
}
