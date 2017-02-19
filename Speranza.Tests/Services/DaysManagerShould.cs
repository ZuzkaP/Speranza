﻿using System;
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
        private const DayNames DAY = DayNames.Wednesday;
        private IDayModel day;
        private Mock<IDatabaseGateway> db;
        private Mock<ITrainingsManager> trainingsManager;
        private Mock<ITrainingModel> trainingModel2;
        private Mock<ITrainingModel> trainingModel1;
        private Mock<IDateTimeService> dateTimeService;
        private const string ID1 = "testID1";
        private const string ID2 = "testID2";
        private const string EMAIL= "testEmail";
        private Mock<IModelFactory> factory;
        private Mock<IRecurringTrainingTemplate> template1;
        private Mock<IRecurringTrainingTemplate> template2;
        private Mock<ITrainingModel> generatedTrainingModel2;
        private Mock<ITrainingModel> generatedTrainingModel1;

        [TestMethod]
        public void ShowEmptyTrainingList_When_NoTrainingExists()
        {
            InitializeDaysManager();
            PrepareDatabaseWithNoTrainings();
            PrepareNoTemplateForTheDay();

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
            PrepareNoTemplateForTheDay();

            RequestDay();

            Assert.AreEqual("2.12.", day.Date);
            Assert.AreEqual(DAY,day.DayName);
                
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
            factory.Setup(r => r.CreateTrainingModel(training1.Object)).Returns(trainingModel1.Object);
            factory.Setup(r => r.CreateTrainingModel(training2.Object)).Returns(trainingModel2.Object);
        }

        [TestMethod]
        public void NotAllowToSignUpToTrainingInPast()
        {
            InitializeDaysManager();
            PrepareTrainingInPast();

            RequestDay();

            trainingModel1.VerifySet(r => r.IsAllowedToSignUp = false);
        }


        [TestMethod]
        public void AllowToSignUp_When_TrainingIsInFuture()
        {
            InitializeDaysManager();
            PrepareTrainingInCloseFuture();

            RequestDay();

            trainingModel1.VerifySet(r => r.IsAllowedToSignUp = true);
        }

        [TestMethod]
        public void NotAllowToSignOffFromTrainingInCloseFuture()
        {
            InitializeDaysManager();
            PrepareTrainingInCloseFuture();

            RequestDay();

            trainingModel1.VerifySet(r => r.IsAllowedToSignOff = false);
            trainingsManager.Verify(r => r.GetSignOffLimit());

        }

        [TestMethod]
        public void AllowToSignOffFromTrainingInMoreDistantFuture()
        {
            InitializeDaysManager();
            PrepareTrainingInDistantFuture();

            RequestDay();

            trainingModel1.VerifySet(r => r.IsAllowedToSignOff = true);
            trainingsManager.Verify(r => r.GetSignOffLimit());
        }

        [TestMethod]
        public void GetDaysTraining_When_NoTemplate()
        {
            InitializeDaysManager();
            PrepareDatabaseWithTwoTrainings();
            PrepareNoTemplateForTheDay();

            RequestDay();

            Assert.AreEqual(2, day.Trainings.Count);
            Assert.AreEqual(trainingModel1.Object, day.Trainings[0]);
            Assert.AreEqual(trainingModel2.Object, day.Trainings[1]);
        }

        [TestMethod]
        public void GetOnlyDayTrainings_When_TemplateExistsAndDayTrainingExistToo()
        {
            InitializeDaysManager();
            PrepareDatabaseWithTwoTrainings();
            PrepareTwoTemplatesForTheDay();

            RequestDay();

            Assert.AreEqual(2, day.Trainings.Count);
            Assert.AreEqual(trainingModel1.Object, day.Trainings[0]);
            Assert.AreEqual(trainingModel2.Object, day.Trainings[1]);
        }

        [TestMethod]
        public void GenerateTrainings_When_NoDayTrainingExistsAndTemplatesDo()
        {
            InitializeDaysManager();
            PrepareDatabaseWithNoTrainings();
            PrepareTwoTemplatesForTheDay();

            RequestDay();

            Assert.AreEqual(2, day.Trainings.Count);
            trainingsManager.Verify(r => r.GenerateTrainingFromTemplate(template1.Object,date));
            trainingsManager.Verify(r => r.GenerateTrainingFromTemplate(template2.Object,date));
            Assert.AreEqual(generatedTrainingModel1.Object, day.Trainings[0]);
            Assert.AreEqual(generatedTrainingModel2.Object, day.Trainings[1]);
        }

        private void PrepareTwoTemplatesForTheDay()
        {
            var templateList = new List<IRecurringTrainingTemplate>();
            template1 = new Mock<IRecurringTrainingTemplate>();
            template2 = new Mock<IRecurringTrainingTemplate>();
            templateList.Add(template1.Object);
            templateList.Add(template2.Object);
            generatedTrainingModel1 = new Mock<ITrainingModel>();
            generatedTrainingModel2 = new Mock<ITrainingModel>();
            db.Setup(r => r.GetTemplatesForTheDay((int)DAY)).Returns(templateList);
            trainingsManager.Setup(r => r.GenerateTrainingFromTemplate(template1.Object,date)).Returns(generatedTrainingModel1.Object);
            trainingsManager.Setup(r => r.GenerateTrainingFromTemplate(template2.Object,date)).Returns(generatedTrainingModel2.Object);
        }

        private void PrepareNoTemplateForTheDay()
        {
            db.Setup(r => r.GetTemplatesForTheDay((int)DAY)).Returns(new List<IRecurringTrainingTemplate>());
        
        }

        private void PrepareTrainingInDistantFuture()
        {
            PrepareDatabaseWithTwoTrainings();
            trainingModel1.Setup(r => r.Time).Returns(new DateTime(2016, 12, 2, 18, 00, 00));
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(date);
        }

        private void PrepareTrainingInCloseFuture()
        {
            PrepareDatabaseWithTwoTrainings();
            trainingModel1.Setup(r => r.Time).Returns(new DateTime(2016, 12, 2, 12, 00, 00));
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
            factory = new Mock<IModelFactory>();
            manager = new DaysManager(db.Object, trainingsManager.Object, dateTimeService.Object, factory.Object);
            dateTimeService.Setup(r => r.GetDayName(date)).Returns(DAY);
            trainingsManager.Setup(r => r.GetSignOffLimit()).Returns(4);


        }
    }
}
