using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Speranza.Models.Interfaces;
using Moq;
using Speranza.Database.Data.Interfaces;
using Speranza.Database.Data;
using Speranza.Common.Data;

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
        private const bool ISADMIN = true;
        private const  UserCategories USERCATEGORY = UserCategories.Gold;
        private const int  NUMBEROFFREESIGNUPS = 5;
        private const int  NUMBEROFSIGNEDUPTRAININGS = 3;
        public const string CATEGORY = "Silver";

        private const int REGISTERED = 8;
        private const int DAY = 5;
        private const int HOUR = 6;
        private const int NUMBEROFPASTRAININGS = 5;
        private Mock<IRecurringTrainingTemplate> template;
        private readonly DateTime DATE = new DateTime(2017,05,16);

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
            Assert.AreEqual(ISADMIN, model.IsAdmin);
            Assert.AreEqual(USERCATEGORY.ToString(), model.Category);
            Assert.AreEqual(NUMBEROFFREESIGNUPS, model.NumberOfFreeSignUps);
            Assert.AreEqual(NUMBEROFSIGNEDUPTRAININGS, model.TrainingCount);
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
        
        [TestMethod]
        public void ReturnTheRightModelForEachTraining()
        {
            InitializeModelFactory();
            PrepareTraining();

            ITrainingModel model = factory.CreateTrainingModel(training.Object);

            Assert.AreEqual(ID, model.ID);
            Assert.AreEqual(TIME, model.Time);
            Assert.AreEqual(CAPACITY, model.Capacity);
            Assert.AreEqual(DESCRIPTION, model.Description);
            Assert.AreEqual(TRAINER, model.Trainer);
            Assert.AreEqual(REGISTERED, model.RegisteredNumber);
            Assert.AreEqual(false, model.IsUserSignedUp);
        }

        [TestMethod]
        public void ReturnUsersInTrainingModel()
        {
            InitializeModelFactory();
            PrepareUserFromDatabase();

            IUserForTrainingDetailModel model = factory.CreateUsersForTrainingDetailModel(user.Object);

            Assert.AreEqual(NAME, model.Name);
            Assert.AreEqual(SURNAME, model.Surname);
            Assert.AreEqual(EMAIL, model.Email);
            Assert.AreEqual(false, model.HasNoAvailableTrainings);
            Assert.AreEqual(true, model.ParticipationSet);
            Assert.AreEqual(DATE, model.SignUpTime);
        }

        [TestMethod]
        public void ReturnUserInUserProfileModel()
        {
            InitializeModelFactory();
            PrepareUserFromDatabase();

            IUserProfileModel result = factory.CreateUserForUserProfileModel(user.Object);
            
            Assert.AreEqual(USERCATEGORY.ToString(), result.Category);
            Assert.AreEqual(NUMBEROFFREESIGNUPS, result.NumberOfFreeSignUps);
            Assert.AreEqual(NUMBEROFPASTRAININGS, result.NumberOfPastTrainings);
            Assert.AreEqual(NAME, result.Name);
            Assert.AreEqual(SURNAME, result.Surname);
            Assert.AreEqual(PHONENUMBER, result.PhoneNumber);
        }
        [TestMethod]
        public void ReturnRecurringTemplateModel()
        {
            InitializeModelFactory();
            PrepareTemplatesFromDB();

            IRecurringTemplateModel model = factory.CreateRecurringTrainingModel(template.Object);

            Assert.AreEqual(DESCRIPTION, model.Description);
            Assert.AreEqual(CAPACITY, model.Capacity);
            Assert.AreEqual(TRAINER, model.Trainer);
            Assert.AreEqual(DAY, model.Day);
            Assert.AreEqual(HOUR, model.Time);
            Assert.AreEqual(DATE, model.ValidFrom);
        }

        private void PrepareTemplatesFromDB()
        {
            template = new Mock<IRecurringTrainingTemplate>();

            template.SetupGet(r => r.Capacity).Returns(CAPACITY);
            template.SetupGet(r => r.Description).Returns(DESCRIPTION);
            template.SetupGet(r => r.Trainer).Returns(TRAINER);
            template.SetupGet(r => r.Day).Returns(DAY);
            template.SetupGet(r => r.Time).Returns(HOUR);
            template.SetupGet(r => r.ValidFrom).Returns(DATE);
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
            user.SetupGet(r => r.IsAdmin).Returns(ISADMIN);
            user.SetupGet(r => r.Category).Returns(USERCATEGORY);
            user.SetupGet(r => r.NumberOfFreeSignUpsOnSeasonTicket).Returns(NUMBEROFFREESIGNUPS);
            user.SetupGet(r => r.NumberOfSignedUpTrainings).Returns(NUMBEROFSIGNEDUPTRAININGS);
            user.SetupGet(r => r.NumberOfPastTrainings).Returns(NUMBEROFPASTRAININGS);
            user.SetupGet(r => r.ParticipationSet).Returns(true);
            user.SetupGet(r => r.SignUpTime).Returns(DATE);
        }

        private void InitializeModelFactory()
        {
            factory = new ModelFactory();
        }
    }
}
