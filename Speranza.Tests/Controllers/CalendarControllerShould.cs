using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using System.Web.Mvc;
using Speranza.Models;
using Moq;
using Speranza.Services;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class CalendarControllerShould
    {
        private CalendarController calendar;
        private Mock<IDateTimeService> dateTimeService;


        [TestMethod]
        public void SendDateInfoToView()
        {
            InitializeController();
            Assert.Fail("TO DO");
           
        }

        private void InitializeController()
        {
            calendar = new CalendarController();
        }
    }
}
