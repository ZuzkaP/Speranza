using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Moq;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;

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

            manager = new TrainingsManager();
        }
    }
}
