using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using Speranza.Services.Interfaces;
using Moq;
using Speranza.Models;
using System.Web.SessionState;
using System.Web.Mvc;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class AccountsControllerForgottenPassShould
    {
        private const string EMAIL = "EMAIL";
        private AccountsController controller;
        private Mock<IUserManager> userManager;

        [TestMethod]
        public void ShowMessage_When_PasswordWasNotChanged()
        {
            InitializeAccountsControllerForgottenPass();
            PreparePasswordNotChanged();

            ActionResult result = controller.SendNewPass(EMAIL);

            userManager.Verify(r => r.SendNewPass(EMAIL), Times.Once);
            Assert.AreEqual(RegisterModelMessages.PasswordRecoveryFailed, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Accounts", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("ForgottenPass", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void RedirectToLogin_When_PassWasChanged()
        {
            InitializeAccountsControllerForgottenPass();
            PreparePasswordChanged();

            ActionResult result = controller.SendNewPass(EMAIL);

            userManager.Verify(r => r.SendNewPass(EMAIL), Times.Once);
            Assert.AreEqual(null, controller.Session["Message"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void SendMessageToUi_AndClear()
        {
            InitializeAccountsControllerForgottenPass();
            PrepareMessage();

            ViewResult result = controller.ForgottenPass();

            Assert.AreEqual(RegisterModelMessages.PasswordRecoveryFailed, result.Model);
            Assert.IsNull(controller.Session["Message"]);

        }

        private void PrepareMessage()
        {
            controller.Session["Message"] = RegisterModelMessages.PasswordRecoveryFailed;
        }

        private void PreparePasswordChanged()
        {
            userManager.Setup(r => r.SendNewPass(EMAIL)).Returns(true);
        }

        private void PreparePasswordNotChanged()
        {
            userManager.Setup(r => r.SendNewPass(EMAIL)).Returns(false);
        }

        private void InitializeAccountsControllerForgottenPass()
        {
            userManager = new Mock<IUserManager>();
            controller = new AccountsController(null, null, userManager.Object, null, null, null);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            controller.ControllerContext = new FakeControllerContext(controller, sessionItems);
        }
    }
}
