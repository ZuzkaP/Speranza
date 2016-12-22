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
        private readonly DateTime date = new DateTime(2016, 12, 2,10,00,00);
        private const DayNames MONDAY = DayNames.Monday;
        private IDayModel day;
        private Mock<IDatabaseGateway> db;
        private Mock<ITrainingsManager> trainingsManager;
        private Mock<ITrainingModel> trainingModel2;
        private Mock<ITrainingModel> trainingModel1;
        private Mock<IDateTimeService> dateTimeService;
        private const string ID1 = "testID1";
        private const string ID2 = "testID2";
        private const string EMAIL= "testEmail";

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

        [TestMethod]
        public void SetDateTimeInformationIntoModel()
        {
            InitializeDaysManager();
            RequestDay();
            Assert.AreEqual("2.12.", day.Date);
            Assert.AreEqual(MONDAY,day.DayName);
                
        }

        [TestMethod]
        public void CorrectlyMarkedTrainings_When_UserIsSignedUpToThem()
        {
            InitializeDaysManager();
            PrepareDatabaseWithTwoTrainings();
            db.Setup(r => r.IsUserAlreadySignedUpInTraining(EMAIL, ID1)).Returns(true);
            db.Setup(r => r.IsUserAlreadySignedUpInTraining(EMAIL, ID2)).Returns(false);

            RequestDay();

            trainingModel1.VerifySet(r => r.IsUserSignedUp = true, Times.Once);
            trainingModel2.VerifySet(r => r.IsUserSignedUp = false, Times.Once);

        }

        private void PrepareDatabaseWithTwoTrainings()
        {
            List<ITraining> trainingsInDB = new List<ITraining>();
            var training1 = new Mock<ITraining>();
            var training2 = new Mock<ITraining>();
            training1.SetupGet(r => r.ID).Returns(ID1);
            training2.SetupGet(r => r.ID).Returns(ID2);
            trainingsInDB.Add(training1.Object);
            trainingsInDB.Add(training2.Object);
            db.Setup(r => r.GetDayTrainings(date)).Returns(trainingsInDB);

            trainingModel1 = new Mock<ITrainingModel>();
            trainingModel2 = new Mock<ITrainingModel>();
            trainingsManager.Setup(r => r.CreateModel(training1.Object)).Returns(trainingModel1.Object);
            trainingsManager.Setup(r => r.CreateModel(training2.Object)).Returns(trainingModel2.Object);
        }

        [TestMethod]
        public void NotAllowToSignUpToTrainingInPast()
        {
            InitializeDaysManager();
            PrepareTrainingInPast();

            RequestDay();

            trainingModel1.VerifySet(r => r.IsAllowedToSignedUp = false);

        }


        [TestMethod]
        public void AllowToSignUp_When_TrainingIsInFuture()
        {
            InitializeDaysManager();
            PrepareTrainingInFuture();

            RequestDay();

            trainingModel1.VerifySet(r => r.IsAllowedToSignedUp = true);

        }

        private void PrepareTrainingInFuture()
        {
            PrepareDatabaseWithTwoTrainings();
            trainingModel1.Setup(r => r.Time).Returns(new DateTime(2016, 12, 2, 18, 00, 00));
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(date);
        }

        private void PrepareTrainingInPast()
        {
            PrepareDatabaseWithTwoTrainings();
            trainingModel1.Setup(r => r.Time).Returns(new DateTime(2016, 12, 2, 8, 00, 00));
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(date);
        }

        private void PrepareDatabaseWithNoTrainings()
        {
            db.Setup(r => r.GetDayTrainings(date)).Returns(new List<ITraining>());
        }

        private void RequestDay()
        {
            day = manager.GetDay(date,EMAIL);
        }

        private void InitializeDaysManager()
        {
            db = new Mock<IDatabaseGateway>();
            trainingsManager = new Mock<ITrainingsManager>();
            dateTimeService = new Mock<IDateTimeService>();
            manager = new DaysManager(db.Object, trainingsManager.Object, dateTimeService.Object);
            dateTimeService.Setup(r => r.GetDayName(date)).Returns(MONDAY);

        }
    }
}
