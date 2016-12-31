using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Speranza.Models.Interfaces;
using Moq;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class ModelFactoryShould
    {
        private ModelFactory factory;
        private Mock<IUser> user;
        private const string NAME = "Miro";
        private const string SURNAME = "Nejaký";
        private const string EMAIL = "test";
        private const string PHONENUMBER = "cislo";
        private Mock<ITraining> training;

        private const string TRAINER ="Dano";
        private readonly DateTime TIME =new DateTime(2017,02,02,10,00,00);
        private const int REGISTEREDNUMBER = 8;
        private const string DESCRIPTION ="test training";
        private const int CAPACITY = 10;
        private const string ID = "ID";

        [TestMethod]
        public void CorrectlyCreateUserForAdminModel()
        {
            InitializeModelFactory();
            PrepareUserFromDatabase();

            IUserForAdminModel model = factory.CreateUserForAdminModel(user.Object);

            Assert.AreEqual(NAME, model.Name);
            Assert.AreEqual(SURNAME, model.Surname);
            Assert.AreEqual(EMAIL, model.Email);
            Assert.AreEqual(PHONENUMBER, model.PhoneNumber);

        }

        [TestMethod]
        public void CorrectlyCreateTrainingsForAdminModel()
        {
            InitializeModelFactory();
            PrepareTrainingFromDatabase();

            ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(training.Object);

            Assert.AreEqual(ID, model.ID);
            Assert.AreEqual(CAPACITY, model.Capacity);
            Assert.AreEqual(DESCRIPTION, model.Description);
            Assert.AreEqual(REGISTEREDNUMBER, model.RegisteredNumber);
            Assert.AreEqual(TIME, model.Time);
            Assert.AreEqual(TRAINER, model.Trainer);
        }

        private void PrepareTrainingFromDatabase()
        {
            training = new Mock<ITraining>();
            training.SetupGet(r => r.ID).Returns(ID);
            training.SetupGet(r => r.Capacity).Returns(CAPACITY);
            training.SetupGet(r => r.Description).Returns(DESCRIPTION);
            training.SetupGet(r => r.RegisteredNumber).Returns(REGISTEREDNUMBER);
            training.SetupGet(r => r.Time).Returns(TIME);
            training.SetupGet(r => r.Trainer).Returns(TRAINER);

        }

        private void PrepareUserFromDatabase()
        {
            user = new Mock<IUser>();
            user.SetupGet(r => r.Name).Returns(NAME);
            user.SetupGet(r => r.Surname).Returns(SURNAME);
            user.SetupGet(r => r.Email).Returns(EMAIL);
            user.SetupGet(r => r.PhoneNumber).Returns(PHONENUMBER);
        }

        private void InitializeModelFactory()
        {
            factory = new ModelFactory();
        }
    }
}
