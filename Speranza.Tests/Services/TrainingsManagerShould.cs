using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Moq;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using Speranza.Database;
using System.Collections.Generic;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class TrainingsManagerShould
    {
        private TrainingsManager manager;
        private Mock<ITraining> training;
        private readonly DateTime TIME = new DateTime(2016,1,1,16,00,00);
        private const string DESCRIPTION = "description";
        private const string TRAINER = "trainer";
        private const string ID = "testID";
        private const int CAPACITY = 10;
        private const int REGISTERED = 8;
        private Mock<IDatabaseGateway> db;
        private Mock<ITraining> training1;
        private Mock<ITraining> training2;
        private Mock<IModelFactory> factory;
        private Mock<ITrainingForAdminModel> training2Model;
        private Mock<ITrainingForAdminModel> training1Model;

        [TestMethod]
        public void ReturnTheRightModelForEachTraining()
        {
            InitializeManager();
            PrepareTraining();

            ITrainingModel model = manager.CreateModel(training.Object);

            Assert.AreEqual(ID, model.ID);
            Assert.AreEqual(TIME, model.Time);
            Assert.AreEqual(CAPACITY, model.Capacity);
            Assert.AreEqual(DESCRIPTION, model.Description);
            Assert.AreEqual(TRAINER, model.Trainer);
            Assert.AreEqual(REGISTERED, model.RegisteredNumber);
            Assert.AreEqual(false, model.IsUserSignedUp);

        }

        [TestMethod]
        public void ReturnEmptyList_When_NoTrainingExistsInDB()
        {
            InitializeManager();
            PrepareDBWithNoTrainings();

           var trainings = manager.GetAllTrainingsForAdmin();

            Assert.AreNotEqual(null, trainings);
            Assert.AreEqual(0, trainings.Count);

        }

        [TestMethod]
        public void ReturnListWithTraining_When_TrainingExistsInDB()
        {
            InitializeManager();
            PrepareDBWithTwoTrainings();
            PrepareFactory();

            var trainings = manager.GetAllTrainingsForAdmin();

            Assert.IsNotNull(trainings);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
            Assert.AreEqual(training2Model.Object, trainings[1]);
        }

        private void PrepareFactory()
        {
            training1Model = new Mock<ITrainingForAdminModel>();
            training2Model = new Mock<ITrainingForAdminModel>();
            factory.Setup(r => r.CreateTrainingForAdminModel(training1.Object)).Returns(training1Model.Object);
            factory.Setup(r => r.CreateTrainingForAdminModel(training2.Object)).Returns(training2Model.Object);
        }

        private void PrepareDBWithTwoTrainings()
        {
            training1 = new Mock<ITraining>();
            training2 = new Mock<ITraining>();
            var trainingsFromDB = new List<ITraining>() { training1.Object,training2.Object};

            db.Setup(r => r.GetAllTrainings()).Returns(trainingsFromDB);
        }

        private void PrepareDBWithNoTrainings()
        {
            db.Setup(r => r.GetAllTrainings()).Returns(new List<ITraining>());
        }

        private void PrepareTraining()
        {
            training = new Mock<ITraining>();

            training.SetupGet(r => r.ID).Returns(ID);
            training.SetupGet(r => r.Time).Returns(TIME);
            training.SetupGet(r => r.Description).Returns(DESCRIPTION);
            training.SetupGet(r => r.Trainer).Returns(TRAINER);
            training.SetupGet(r => r.Capacity).Returns(CAPACITY);
            training.SetupGet(r => r.RegisteredNumber).Returns(REGISTERED);

        }

        private void InitializeManager()
        {
            db = new Mock<IDatabaseGateway>();
            factory = new Mock<IModelFactory>();
            manager = new TrainingsManager(db.Object,factory.Object);
        }
    }
}
