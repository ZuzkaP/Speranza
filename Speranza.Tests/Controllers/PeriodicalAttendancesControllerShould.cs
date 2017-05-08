using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using System.Web.Mvc;
using Moq;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class PeriodicalAttendancesControllerShould
    {
        private PeriodicalAttendancesController controller;
        private Mock<IUserManager> userManager;

        [TestMethod]
        public void ReturnEmptyResult()
        {
            InitializePeriodicalController();

            ActionResult result = controller.Execute();

            Assert.IsInstanceOfType(result, typeof(EmptyResult));
        }

        [TestMethod]
        public void CallUserManagerToUpdateSeasonTicket()
        {
            InitializePeriodicalController();

            controller.Execute();

            userManager.Verify(r => r.UpdateSeasonTickets(), Times.Once);
        }

        [TestMethod]
        public void CallUserManagerToConfirmUsersAttendance()
        {
            InitializePeriodicalController();

            controller.Execute();

            userManager.Verify(r => r.PromptToConfirmUserAttendance(), Times.Once);
        }

        [TestMethod]
        public void CallUserManagerToCleanUpTokens()
        {
            InitializePeriodicalController();

            controller.Execute();

            userManager.Verify(r => r.CleanUpTokens(), Times.Once);
        }

        private void InitializePeriodicalController()
        {
            userManager = new Mock<IUserManager>();
            controller = new PeriodicalAttendancesController(userManager.Object);
        }
    }
}
