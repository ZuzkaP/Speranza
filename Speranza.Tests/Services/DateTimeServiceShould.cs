using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;

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

        private void InitializeService()
        {
            service = new DateTimeService();
        }
    }
}
