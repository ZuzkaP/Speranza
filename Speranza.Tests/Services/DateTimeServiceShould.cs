using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces.Exceptions;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class DateTimeServiceShould
    {
        private DateTimeService service;
        private const string TIME = "05:00";
        private const string DATE = "01.01.2017";
        private const string INVALID_DATE = "jozko";
        private string INVALID_TIME = "janko";
        private readonly DateTime EXPECTED_DATETIME = new DateTime(2017,01,01,05,00,00);

        [TestMethod]
        public void ReturnTodaysDate()
        {
            InitializeService();
            DateTime expected = DateTime.Now;
            DateTime recieved = service.GetCurrentDateTime();
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

        [TestMethod]
        [ExpectedException(typeof(InvalidDateException))]
        public void ThrowException_When_DateIsEmpty()
        {
            InitializeService();

            service.ParseDateTime(string.Empty, TIME);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidDateException))]
        public void ThrowException_When_DateIsInvalid()
        {
            InitializeService();

            service.ParseDateTime(INVALID_DATE, TIME);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidTimeException))]
        public void ThrowException_When_TimeIsEmpty()
        {
            InitializeService();

            service.ParseDateTime(DATE, string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidTimeException))]
        public void ThrowException_When_TimeIsInvalid()
        {
            InitializeService();

            service.ParseDateTime(DATE, INVALID_TIME);
        }

        [TestMethod]
        public void ParseDateAndTime()
        {
            InitializeService();

             DateTime resultDateTime = service.ParseDateTime(DATE, TIME);

            Assert.AreEqual(EXPECTED_DATETIME, resultDateTime);
        }

       
        private void InitializeService()
        {
            service = new DateTimeService();
        }
    }
}
