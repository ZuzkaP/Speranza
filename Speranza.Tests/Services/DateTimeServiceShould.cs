using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Speranza.Models.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class DateTimeServiceShould
    {
        private DateTimeService service;

        [TestMethod]
        public void ReturnTodaysDate()
        {
            InitializeService();
            DateTime expected = DateTime.Now;
            DateTime recieved = service.GetCurrentDate();
            Assert.AreEqual(expected.Year, recieved.Year);
            Assert.AreEqual(expected.Month, recieved.Month);
            Assert.AreEqual(expected.Day, recieved.Day);
            Assert.AreEqual(expected.Hour, recieved.Hour);
            Assert.AreEqual(expected.Minute, recieved.Minute);
            Assert.AreEqual(expected.Second, recieved.Second);
        }

        [TestMethod]
        public void ReturnTheRightDayName()
        {
            InitializeService();
            Assert.AreEqual(DayNames.Sunday, service.GetDayName(new DateTime(2016, 12, 4)));
            Assert.AreEqual(DayNames.Monday, service.GetDayName(new DateTime(2016, 12, 5)));
            Assert.AreEqual(DayNames.Friday, service.GetDayName(new DateTime(2016, 12, 9)));

        }

        private void InitializeService()
        {
            service = new DateTimeService();
        }
    }
}
