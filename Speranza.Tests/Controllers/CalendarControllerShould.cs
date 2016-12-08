using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using System.Web.Mvc;
using Speranza.Models;
using Moq;
using Speranza.Services;
using System.Web.SessionState;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class CalendarControllerShould
    {
        private CalendarController calendar;
        private Mock<IUserManager> userManager;
        private const int DAYS_COUNT_STANDARD_USER = 14;
        private const int DAYS_COUNT_SILVER_USER = 30;
        private const int DAYS_COUNT_GOLDEN_USER = 60;
        private Mock<IDaysManager> daysManager;
        private Mock<IDateTimeService> dateTimeService;

        private readonly DateTime CURRENTDATE = new DateTime(2016, 11, 15);
       

        [TestMethod]
        public void ReturnToLogin_When_UserIsNotLoggedIn()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(false);
            ActionResult result = calendar.Calendar();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            
            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        [TestMethod]
        public void Display14days_When_StandardUserIsLoggedIn()
        {
            InitializeController();
            StandardUserIsLoggedIn();

            ActionResult result = calendar.Calendar();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            CalendarModel model = (CalendarModel) ((ViewResult) result).Model;
            Assert.AreEqual(DAYS_COUNT_STANDARD_USER, model.Days.Count);
            for (int i = 0; i < DAYS_COUNT_STANDARD_USER; i++)
            {
                Assert.IsNotNull(model.Days[i]);
                daysManager.Verify(r => r.GetDay(CURRENTDATE + TimeSpan.FromDays(i)), Times.Once);
            }
        }

        [TestMethod]
        public void Display30days_When_SilverUserIsLoggedIn()
        {
            InitializeController();
            SilverUserIsLoggedIn();

            ActionResult result = calendar.Calendar();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            CalendarModel model = (CalendarModel)((ViewResult)result).Model;
            Assert.AreEqual(DAYS_COUNT_SILVER_USER, model.Days.Count);
            for (int i = 0; i < DAYS_COUNT_SILVER_USER; i++)
            {
                Assert.IsNotNull(model.Days[i]);
                daysManager.Verify(r => r.GetDay(CURRENTDATE + TimeSpan.FromDays(i)), Times.Once);
            }
        }

        [TestMethod]
        public void Display60days_When_GoldenUserIsLoggedIn()
        {
            InitializeController();
            GoldenUserIsLoggedIn();

            ActionResult result = calendar.Calendar();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            CalendarModel model = (CalendarModel)((ViewResult)result).Model;
            Assert.AreEqual(DAYS_COUNT_GOLDEN_USER, model.Days.Count);
            for (int i = 0; i < DAYS_COUNT_GOLDEN_USER; i++)
            {
                Assert.IsNotNull(model.Days[i]);
                daysManager.Verify(r => r.GetDay(CURRENTDATE + TimeSpan.FromDays(i)), Times.Once);
            }
        }


        private void GoldenUserIsLoggedIn()
        {
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(true);
            userManager.Setup(r => r.GetUserCategory(calendar.Session)).Returns(UserCategories.Gold);
        }

        private void SilverUserIsLoggedIn()
        {
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(true);
            userManager.Setup(r => r.GetUserCategory(calendar.Session)).Returns(UserCategories.Silver);
        }

        private void StandardUserIsLoggedIn()
        {
            userManager.Setup(r => r.IsUserLoggedIn(calendar.Session)).Returns(true);
            userManager.Setup(r => r.GetUserCategory(calendar.Session)).Returns(UserCategories.Standard);
        }

        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            daysManager = new Mock<IDaysManager>();
            dateTimeService = new Mock<IDateTimeService>();
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(CURRENTDATE);
            calendar = new CalendarController(null,userManager.Object,daysManager.Object,dateTimeService.Object);
            for (int i = 0; i < 60; i++)
            {
                daysManager.Setup(r => r.GetDay(CURRENTDATE + TimeSpan.FromDays(i))).Returns(new Mock<IDayModel>().Object);
            }
          
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            calendar.ControllerContext = new FakeControllerContext(calendar, sessionItems);
           
        }
    }
}
