using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Models;
using Speranza.Services;
using Moq;
using Speranza.Database;
using System.Collections.Generic;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class DaysManagerShould
    {
        private DaysManager manager;
        private readonly DateTime date = new DateTime(2016, 12, 2);
        private IDayModel day;
        private Mock<IDatabaseGateway> db;

        [TestMethod]
        public void ShowEmptyTrainingList_When_NoTrainingExists()
        {
            InitializeDaysManager();
            PrepareDatabaseWithNoTrainings();

            RequestDay();

            Assert.IsNotNull(day.Trainings);
            Assert.AreEqual(0, day.Trainings.Count);
        }

        private void PrepareDatabaseWithNoTrainings()
        {
            db.Setup(r => r.GetDayTrainings(date)).Returns(new List<ITraining>());
        }

        private void RequestDay()
        {
            day = manager.GetDay(date);
        }

        private void InitializeDaysManager()
        {
            manager = new DaysManager();
            db = new Mock<IDatabaseGateway>();
        }
    }
}
