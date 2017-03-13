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

        private void InitializePeriodicalController()
        {
            userManager = new Mock<IUserManager>();
            controller = new PeriodicalAttendancesController(userManager.Object);
        }
    }
}
