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
        private Mock<IDatabaseGateway> db;
        private Mock<ITraining> training1;
        private Mock<ITraining> training2;
        private Mock<IModelFactory> factory;
        private Mock<ITrainingForAdminModel> training2Model;
        private Mock<ITrainingForAdminModel> training1Model;
        private const string EMAIL = "test";
        private const string TRAINING_ID = "testID";

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

        [TestMethod]
        public void RemoveUserFromTraining()
        {
            InitializeManager();

            manager.RemoveUserFromTraining(EMAIL, ID);

            db.Verify(r => r.RemoveUserFromTraining(EMAIL, TRAINING_ID));

        }

        [TestMethod]
        public void RemoveUserFromTraining_And_ReturnModel()
        {
            InitializeManager();
            var factoryModel = new Mock<ITrainingModel>();
            var training = new Mock<ITraining>();
            db.Setup(r => r.GetTrainingData(ID)).Returns(training.Object);
            factory.Setup(r => r.CreateTrainingModel(training.Object)).Returns(factoryModel.Object);
           
            ITrainingModel model = manager.RemoveUserFromTraining(EMAIL, ID);

            Assert.AreEqual(factoryModel.Object, model);


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
        
        private void InitializeManager()
        {
            db = new Mock<IDatabaseGateway>();
            factory = new Mock<IModelFactory>();
            manager = new TrainingsManager(db.Object,factory.Object);
        }
    }
}
