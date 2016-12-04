using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Models;
using Speranza.Services;
using Moq;
using Speranza.Database;
using System.Collections.Generic;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class DaysManagerShould
    {
        private DaysManager manager;
        private readonly DateTime date = new DateTime(2016, 12, 2);
        private IDayModel day;
        private Mock<IDatabaseGateway> db;
        private Mock<ITrainingsManager> trainingsManager;
        private Mock<ITrainingModel> trainingModel2;
        private Mock<ITrainingModel> trainingModel1;

        [TestMethod]
        public void ShowEmptyTrainingList_When_NoTrainingExists()
        {
            InitializeDaysManager();
            PrepareDatabaseWithNoTrainings();

            RequestDay();

            Assert.IsNotNull(day.Trainings);
            Assert.AreEqual(0, day.Trainings.Count);
        }

        [TestMethod]
        public void ShowTrainings_When_DBContainsTrainingsOnThisDay()
        {
            InitializeDaysManager();
            PrepareDatabaseWithTwoTrainings();

            RequestDay();
            Assert.IsNotNull(day.Trainings);
            Assert.AreEqual(2, day.Trainings.Count);
            Assert.AreEqual(trainingModel1.Object, day.Trainings[0]);
            Assert.AreEqual(trainingModel2.Object, day.Trainings[1]);

        }

        private void PrepareDatabaseWithTwoTrainings()
        {
            List<ITraining> trainingsInDB = new List<ITraining>();
            var training1 = new Mock<ITraining>();
            var training2 = new Mock<ITraining>();
            trainingsInDB.Add(training1.Object);
            trainingsInDB.Add(training2.Object);
            db.Setup(r => r.GetDayTrainings(date)).Returns(trainingsInDB);

            trainingModel1 = new Mock<ITrainingModel>();
            trainingModel2 = new Mock<ITrainingModel>();
            trainingsManager.Setup(r => r.CreateModel(training1.Object)).Returns(trainingModel1.Object);
            trainingsManager.Setup(r => r.CreateModel(training2.Object)).Returns(trainingModel2.Object);
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
            db = new Mock<IDatabaseGateway>();
            trainingsManager = new Mock<ITrainingsManager>();
            manager = new DaysManager(db.Object,trainingsManager.Object);
        }
    }
}
