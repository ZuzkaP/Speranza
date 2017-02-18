using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Moq;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using Speranza.Database;
using System.Collections.Generic;
using Speranza.Services.Interfaces;
using Speranza.Services.Interfaces.Exceptions;
using Speranza.Models;
using Speranza.Database.Data;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class TrainingsManagerShould
    {
        private TrainingsManager manager;
        private readonly DateTime DATE_TIME = new DateTime(2016,1,1,16,00,00);
        private const string DESCRIPTION = "description";
        private const string TRAINER = "trainer";
        private const int CAPACITY = 10;
        private Mock<IDatabaseGateway> db;
        private Mock<ITraining> training1;
        private Mock<ITraining> training2;
        private Mock<IModelFactory> factory;
        private Mock<ITrainingForAdminModel> training2Model;
        private Mock<ITrainingForAdminModel> training1Model;
        private const string EMAIL = "test";
        private const string TRAINING_ID = "testID";
        private const string FALSE_TRAINING_ID = "FalseTestID";
        private Mock<IUserForTrainingDetailModel> user2Model;
        private Mock<IUserForTrainingDetailModel> user1Model;
        private const string TRAINING_DESCRIPTION = "description";
        private const int TRAINING_CAPACITY = 10;
        private Mock<IUidService> uidService;
        private readonly DateTime DATE_IN_PAST = new DateTime(2016,12,12);
        private readonly DateTime DATE_IN_FUTURE = new DateTime(2017, 12, 12);
        private readonly DateTime CURRENT_DATE = new DateTime(2017, 01, 12);
        private Mock<IDateTimeService> dateTimeService;
        private const int HOURS_LIMIT = 12;
        private const string FALSE_EMAIL = "falseEmail";
        private Mock<IUserManager> userManager;
        private RecurringModel model;
        private List<bool> checkedTrainings;
        private const int DAY_B = 5;
        private const int DAY_A = 6;
        private const int TIME_B = 13;
        private const int TIME_A = 19;
        private Mock<IRecurringTemplateModel> modelB;
        private Mock<IRecurringTemplateModel> modelA;

        [TestMethod]
        public void ReturnEmptyList_When_NoTrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithNoTrainings();

           var trainings = manager.GetAllFutureTrainings();

            Assert.AreNotEqual(null, trainings);
            Assert.AreEqual(0, trainings.Count);

        }

        [TestMethod]
        public void ReturnListWithTraining_When_TrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithTwoTrainings();
            PrepareFactory();

            var trainings = manager.GetAllFutureTrainings();

            Assert.IsNotNull(trainings);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
            Assert.AreEqual(training2Model.Object, trainings[1]);
        }
        

        [TestMethod]
        public void GetOnlyFutureTrainingsFromDB()
        {
            InitializeTrainingManager();
            PrepareDBWithOnePastAndOneFutureTraining();
            PrepareFactory();

            var trainings = manager.GetAllFutureTrainings();

            Assert.AreEqual(1, trainings.Count);
            Assert.AreEqual(training2Model.Object, trainings[0]);
            
        }

        [TestMethod]
        public void RemoveUserFromTraining()
        {
            InitializeTrainingManager();

            manager.RemoveUserFromTraining(EMAIL,TRAINING_ID);

            db.Verify(r => r.RemoveUserFromTraining(EMAIL, TRAINING_ID));

        }

        [TestMethod]
        public void RemoveUserFromTraining_And_ReturnModel()
        {
            InitializeTrainingManager();
            var factoryModel = new Mock<ITrainingModel>();
            var training = new Mock<ITraining>();
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);
            factory.Setup(r => r.CreateTrainingModel(training.Object)).Returns(factoryModel.Object);
           
            ITrainingModel model = manager.RemoveUserFromTraining(EMAIL, TRAINING_ID);

            Assert.AreEqual(factoryModel.Object, model);
        }

        [TestMethod]
        public void SetTrainer()
        {
            InitializeTrainingManager();

            manager.SetTrainer(TRAINING_ID, TRAINER);

            db.Verify(r => r.SetTrainer(TRAINING_ID, TRAINER), Times.Once);
        }

        [TestMethod]
        public void SetTrainingDescription()
        {
            InitializeTrainingManager();

            manager.SetTrainingDescription(TRAINING_ID, TRAINING_DESCRIPTION);

            db.Verify(r => r.SetTrainingDescription(TRAINING_ID, TRAINING_DESCRIPTION), Times.Once);
        }

        [TestMethod]
        public void SetTrainingCapacity()
        {
            InitializeTrainingManager();

            manager.SetTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY);

            db.Verify(r => r.SetTrainingCapacity(TRAINING_ID, TRAINING_CAPACITY), Times.Once);
        }

        [TestMethod]
        public void GetUsersInTrainingFromDB()
        {
            InitializeTrainingManager();
            PrepareDBAndFactoryWithTwoUsers();

            var users = manager.GetAllUsersInTraining(TRAINING_ID);

            Assert.AreEqual(2, users.Count);
            Assert.AreEqual(user1Model.Object, users[0]);
            Assert.AreEqual(user2Model.Object, users[1]);
        }

        [TestMethod]
        public void CreateNewTraning()
        {
            InitializeTrainingManager();
            uidService.Setup(r => r.CreateID()).Returns(TRAINING_ID);

            string trainingID = manager.CreateNewTraining(DATE_TIME, TRAINER, DESCRIPTION,CAPACITY);

            db.Verify(r => r.CreateNewTraining(TRAINING_ID,DATE_TIME, TRAINER, DESCRIPTION,CAPACITY), Times.Once);
            Assert.AreEqual(TRAINING_ID, trainingID);

        }

        [TestMethod]
        public void CancelTraining()
        {
            InitializeTrainingManager();

             manager.CancelTraining(TRAINING_ID);

            db.Verify(r => r.CancelTraining(TRAINING_ID), Times.Once);
        }

        [TestMethod]
        public void ChangeSignOffLimit()
        {
            InitializeTrainingManager();

            manager.SetSignOffLimit(HOURS_LIMIT);

            db.Verify(r => r.SetSignOffLimit(HOURS_LIMIT), Times.Once);
        }

        [TestMethod]
        public void GetSignOffLimitFromDb()
        {
            InitializeTrainingManager();
            PrepareDBWithSignOffSettings();

            var limit = manager.GetSignOffLimit();

            Assert.AreEqual(HOURS_LIMIT, limit);
        }

        
       [TestMethod]
        public void NotAddUserToTraining_When_UserIsNotInDB()
        {
            InitializeTrainingManager();

            var message = manager.AddUserToTraining(FALSE_EMAIL, TRAINING_ID, CURRENT_DATE);

            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual(CalendarMessages.UserDoesNotExist, message);
        }

        [TestMethod]
        public void NotAddUserToTraining_When_TrainingDoesNotExist()
        {
            InitializeTrainingManager();

           var message = manager.AddUserToTraining(EMAIL, FALSE_TRAINING_ID, CURRENT_DATE);

            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual(CalendarMessages.TrainingDoesNotExist, message);
        }

        [TestMethod]
        public void NotAddUserToTraining_When_TooManyPeopleInTrainingExist()
        {
            InitializeTrainingManager();
            PrepareTrainingWithTooManySignedUpUsers();

            var message = manager.AddUserToTraining(EMAIL, TRAINING_ID, CURRENT_DATE);

            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual(CalendarMessages.TrainingIsFull, message);
        }

        [TestMethod]
        public void NotAddUserToTraining_When_UserAlreadySignedUpInTraining()
        {
            InitializeTrainingManager();
            db.Setup(r => r.IsUserAlreadySignedUpInTraining(EMAIL, TRAINING_ID)).Returns(true);

            var message = manager.AddUserToTraining(EMAIL, TRAINING_ID, CURRENT_DATE);

            db.Verify(r => r.AddUserToTraining(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual(CalendarMessages.UserAlreadySignedUp, message);
        }

        private void PrepareTrainingWithTooManySignedUpUsers()
        {
            training1.SetupGet(r => r.RegisteredNumber).Returns(10);

        }

        [TestMethod]
        public void AddUserToTraining()
        {
            InitializeTrainingManager();

            var message = manager.AddUserToTraining(EMAIL,TRAINING_ID,CURRENT_DATE);

            db.Verify(r => r.AddUserToTraining(EMAIL,TRAINING_ID,CURRENT_DATE), Times.Once);
            Assert.AreEqual(CalendarMessages.SignUpSuccessful, message);

        }
        
        [TestMethod]
        public void NotAddRecurringTrainingIntoDB_When_NoCheckedTimeslot()
        {
            InitializeTrainingManager();
            PrepareModelWithNoCheckedTimeslot();

            manager.CreateRecurringTraining(model);

            db.Verify(r => r.CreateRecurringTrainingTemplate(It.IsAny<RecurringTrainingTemplate>()), Times.Never);
        }


        [TestMethod]
        public void AddTwoRecurringTrainingsIntoDB_When_TwoCheckedTimeslotsExist()
        {
            InitializeTrainingManager();
            PrepareModelWithTwoCheckedTimeslots();

            manager.CreateRecurringTraining(model);

            db.Verify(p => p.CreateRecurringTrainingTemplate(It.Is<RecurringTrainingTemplate>(r=>r.Trainer == TRAINER && r.Capacity == CAPACITY && r.Description == DESCRIPTION && r.Day == DAY_A && r.Time == TIME_A)), Times.Once);
            db.Verify(p => p.CreateRecurringTrainingTemplate(It.Is<RecurringTrainingTemplate>(r=>r.Trainer == TRAINER && r.Capacity == CAPACITY && r.Description == DESCRIPTION && r.Day == DAY_B && r.Time == TIME_B)), Times.Once);
        }


        [TestMethod]
        public void ReturnEmptyList_When_NoTemplateInDBExists()
        {
            InitializeTrainingManager();
            PrepareDBWithNoTemplate();

            var templates = manager.GetTemplates();

            Assert.IsNotNull(templates);
            Assert.AreEqual(0, templates.Count);
        }

        [TestMethod]
        public void ReturnListWithTwoModels_When_TwoTemplatesInDBExist()
        {
            InitializeTrainingManager();
            PrepareDBAndFactoryWithTwoTemplates();

            var templates = manager.GetTemplates();

            Assert.IsNotNull(templates);
            Assert.AreEqual(0, templates.Count);
        }


        [TestMethod]
        public void RemoveTrainingTemplateFromDB()
        {
            InitializeTrainingManager();

            manager.RemoveTrainingTemplate(DAY_A, TIME_A);

            db.Verify(r => r.RemoveTrainingTemplate(DAY_A, TIME_A), Times.Once);

        }

        private void PrepareDBAndFactoryWithTwoTemplates()
        {
            var list = new List<IRecurringTrainingTemplate>();
            var templateA = new Mock<IRecurringTrainingTemplate>();
            var templateB = new Mock<IRecurringTrainingTemplate>();
            list.Add(templateA.Object);
            list.Add(templateB.Object);
            db.Setup(r => r.GetTemplates()).Returns(new List<IRecurringTrainingTemplate>());

            modelA = new Mock<IRecurringTemplateModel>();
            modelB = new Mock<IRecurringTemplateModel>();
            factory.Setup(r => r.CreateRecurringTrainingModel(templateA.Object)).Returns(modelA.Object);
            factory.Setup(r => r.CreateRecurringTrainingModel(templateB.Object)).Returns(modelB.Object);
        }

        private void PrepareDBWithNoTemplate()
        {
            db.Setup(r => r.GetTemplates()).Returns(new List<IRecurringTrainingTemplate>());
        }

        private void PrepareModelWithTwoCheckedTimeslots()
        {
            PrepareModelWithNoCheckedTimeslot();
            model.IsTrainingInTime[DAY_A*13 + TIME_A - 7] = true;
            model.IsTrainingInTime[DAY_B*13 + TIME_B - 7] = true;
        }

        private void PrepareModelWithNoCheckedTimeslot()
        {
            model = new RecurringModel();
            model.Capacity = CAPACITY;
            model.Description = DESCRIPTION;
            model.Trainer = TRAINER;
            PrepareArrayWithNoCheckedTraining();
            model.IsTrainingInTime = checkedTrainings;
        }
        
        private void PrepareArrayWithNoCheckedTraining()
        {
            checkedTrainings = new List<bool>();
            for (int i = 0; i < 7 * 13; i++)
            {
                checkedTrainings.Add(false);
            }
        }

        private void PrepareDBWithSignOffSettings()
        {
            db.Setup(r => r.GetSignOffLimit()).Returns(HOURS_LIMIT);
        }

        private void PrepareDBAndFactoryWithTwoUsers()
        {
            var user1 = new Mock<IUser>();
            var user2 = new Mock<IUser>();
            var users = new List<IUser>() { user1.Object, user2.Object };
            db.Setup(r => r.GetUsersInTraining(TRAINING_ID)).Returns(users);

            user1Model = new Mock<IUserForTrainingDetailModel>();
            user2Model = new Mock<IUserForTrainingDetailModel>();

            factory.Setup(r => r.CreateUsersForTrainingDetailModel(user1.Object)).Returns(user1Model.Object);
            factory.Setup(r => r.CreateUsersForTrainingDetailModel(user2.Object)).Returns(user2Model.Object);
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
            training1.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            training2.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            var trainingsFromDB = new List<ITraining>() { training1.Object,training2.Object};

            db.Setup(r => r.GetAllTrainings()).Returns(trainingsFromDB);
        }

        private void PrepareDBWithOnePastAndOneFutureTraining()
        {
            training1 = new Mock<ITraining>();
            training1.Setup(r => r.Time).Returns(DATE_IN_PAST);
            training2 = new Mock<ITraining>();
            training2.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            var trainingsFromDB = new List<ITraining>() { training1.Object, training2.Object };

            db.Setup(r => r.GetAllTrainings()).Returns(trainingsFromDB);
        }

        private void PrepareDBWithNoTrainings()
        {
            db.Setup(r => r.GetAllTrainings()).Returns(new List<ITraining>());
        }
        

        private void InitializeTrainingManager()
        {
            db = new Mock<IDatabaseGateway>();
            factory = new Mock<IModelFactory>();
            uidService = new Mock<IUidService>();
            dateTimeService = new Mock<IDateTimeService>();
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(CURRENT_DATE);
            userManager = new Mock<IUserManager>();
            manager = new TrainingsManager(db.Object,factory.Object,uidService.Object,dateTimeService.Object,userManager.Object);

            userManager.Setup(r => r.UserExists(EMAIL)).Returns(true);
            userManager.Setup(r => r.UserExists(FALSE_EMAIL)).Returns(false);
            training1 = new Mock<ITraining>();
            training1.SetupGet(r => r.Capacity).Returns(10);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training1.Object);
            db.Setup(r => r.GetTrainingData(FALSE_TRAINING_ID)).Returns((ITraining)null);
        }
    }
}
