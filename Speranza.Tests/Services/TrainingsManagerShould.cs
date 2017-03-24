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
        private const int TRAININGS_COUNT = 42;
        private const string EMAIL2 = "EMAIL2";
        private Mock<IRecurringTemplateModel> modelB;
        private Mock<IRecurringTemplateModel> modelA;
        private Mock<IRecurringTrainingTemplate> template;
        private Mock<ITrainingModel> trainingModel;
        private Mock<ITrainingForAdminModel> training3Model;
        private Mock<ITraining> training3;
        private Mock<IEmailManager> emailManager;
        private List<IUser> admins;

        [TestMethod]
        public void ReturnEmptyList_When_NoFutureTrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithNoTrainings();

           var trainings = manager.GetFutureTrainings(0,2);

            Assert.AreNotEqual(null, trainings);
            Assert.AreEqual(0, trainings.Count);
        }

        [TestMethod]
        public void ReturnEmptyList_When_NoPastTrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithNoTrainings();

            var trainings = manager.GetPastTrainings(0, 2);

            Assert.AreNotEqual(null, trainings);
            Assert.AreEqual(0, trainings.Count);
        }

        [TestMethod]
        public void ReturnListWithFutureTraining_When_TrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithTwoFutureTrainings();
            PrepareFactory();

            var trainings = manager.GetFutureTrainings(0,2);

            Assert.IsNotNull(trainings);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
            Assert.AreEqual(training2Model.Object, trainings[1]);
        }

        [TestMethod]
        public void ReturnListWithPastTraining_When_TrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithTwoPastTrainings();
            PrepareFactory();

            var trainings = manager.GetPastTrainings(0, 2);

            Assert.IsNotNull(trainings);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
            Assert.AreEqual(training2Model.Object, trainings[1]);
        }

        

        [TestMethod]
        public void ReturnListWithFutureTrainingsFromRange_When_TrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithThreeFutureTrainings();
            PrepareFactory();

            var trainings = manager.GetFutureTrainings(0, 2);

            Assert.IsNotNull(trainings);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
            Assert.AreEqual(training2Model.Object, trainings[1]);
        }

        [TestMethod]
        public void ReturnListWithPastTrainingsFromRange_When_TrainingExistsInDB()
        {
            InitializeTrainingManager();
            PrepareDBWithThreePastTrainings();
            PrepareFactory();

            var trainings = manager.GetPastTrainings(0, 2);

            Assert.IsNotNull(trainings);
            Assert.AreEqual(2, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
            Assert.AreEqual(training2Model.Object, trainings[1]);
        }

        private void PrepareDBWithThreePastTrainings()
        {
            training1 = new Mock<ITraining>();
            training2 = new Mock<ITraining>();
            training3 = new Mock<ITraining>();
            training1.Setup(r => r.Time).Returns(DATE_IN_PAST);
            training2.Setup(r => r.Time).Returns(DATE_IN_PAST);
            training3.Setup(r => r.Time).Returns(DATE_IN_PAST);
            var trainingsFromDB = new List<ITraining>() { training1.Object, training2.Object, training3.Object };

            db.Setup(r => r.GetAllTrainings()).Returns(trainingsFromDB);
        }

        private void PrepareDBWithThreeFutureTrainings()
        {
            training1 = new Mock<ITraining>();
            training2 = new Mock<ITraining>();
            training3 = new Mock<ITraining>();
            training1.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            training2.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            training3.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            var trainingsFromDB = new List<ITraining>() { training1.Object, training2.Object , training3.Object};

            db.Setup(r => r.GetAllTrainings()).Returns(trainingsFromDB);
        }

        [TestMethod]
        public void GetOnlyFutureTrainingsFromDB()
        {
            InitializeTrainingManager();
            PrepareDBWithOnePastAndOneFutureTraining();
            PrepareFactory();

            var trainings = manager.GetFutureTrainings(0,2);

            Assert.AreEqual(1, trainings.Count);
            Assert.AreEqual(training2Model.Object, trainings[0]);
        }

        [TestMethod]
        public void GetOnlyPastTrainingsFromDB()
        {
            InitializeTrainingManager();
            PrepareDBWithOnePastAndOneFutureTraining();
            PrepareFactory();

            var trainings = manager.GetPastTrainings(0, 2);

            Assert.AreEqual(1, trainings.Count);
            Assert.AreEqual(training1Model.Object, trainings[0]);
        }

        [TestMethod]
        public void RemoveUserFromTraining()
        {
            InitializeTrainingManager();

            manager.RemoveUserFromTraining(EMAIL,TRAINING_ID);

            db.Verify(r => r.RemoveUserFromTraining(EMAIL, TRAINING_ID));

        }

        [TestMethod]
        public void RemoveUserFromTraining_And_ReturnModel_AndSendEmail_When_UserIsAdmin()
        {
            InitializeTrainingManager();
            var factoryModel = new Mock<ITrainingModel>();
            var training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);
            factory.Setup(r => r.CreateTrainingModel(training.Object)).Returns(factoryModel.Object);
           
            ITrainingModel model = manager.RemoveUserFromTraining(EMAIL, TRAINING_ID, true);

            Assert.AreEqual(factoryModel.Object, model);
            emailManager.Verify(r => r.SendRemovingUserFromTraining(EMAIL, DATE_TIME), Times.Once);
        }

        [TestMethod]
        public void RemoveUserFromTraining_And_ReturnModel_AndNotSendEmail_When_UserIsNotAdmin()
        {
            InitializeTrainingManager();
            var factoryModel = new Mock<ITrainingModel>();
            var training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);
            factory.Setup(r => r.CreateTrainingModel(training.Object)).Returns(factoryModel.Object);

            ITrainingModel model = manager.RemoveUserFromTraining(EMAIL, TRAINING_ID);

            Assert.AreEqual(factoryModel.Object, model);
            emailManager.Verify(r => r.SendRemovingUserFromTraining(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
        }

        [TestMethod]
        public void ReturnFutureTrainingsCount()
        {
            InitializeTrainingManager();
            PrepareDBWithFutureTrainingsCount();

            var trainings = manager.GetFutureTrainingsCount();

            Assert.AreEqual(TRAININGS_COUNT, trainings);
        }

        [TestMethod]
        public void ReturnPastTrainingsCount()
        {
            InitializeTrainingManager();
            PrepareDBWithPastTrainingsCount();

            var trainings = manager.GetPastTrainingsCount();

            Assert.AreEqual(TRAININGS_COUNT, trainings);
        }

        private void PrepareDBWithPastTrainingsCount()
        {
            db.Setup(r => r.GetTrainingsCountBeforeDate(CURRENT_DATE)).Returns(TRAININGS_COUNT);
        }

        private void PrepareDBWithFutureTrainingsCount()
        {
            db.Setup(r => r.GetTrainingsCountAfterDate(CURRENT_DATE)).Returns(TRAININGS_COUNT);
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
        public void SendEmailToUsersInTraining_When_Cancelled()
        {
            InitializeTrainingManager();
            PrepareTrainingWithTwoUsers();

            manager.CancelTraining(TRAINING_ID);

            emailManager.Verify(r => r.SendTrainingCanceled(EMAIL,DATE_TIME), Times.Once);
            emailManager.Verify(r => r.SendTrainingCanceled(EMAIL2,DATE_TIME), Times.Once);
        }

        [TestMethod]
        public void SendEmailToAdmins_When_SixthUserWasSignedUp()
        {
            InitializeTrainingManager();
            PrepareTrainingWith5Users();
            PrepareAdmins();

            manager.AddUserToTraining(EMAIL,TRAINING_ID,CURRENT_DATE,false);

            emailManager.Verify(r => r.SendSixthUserInTraining(admins, DATE_TIME), Times.Once);
        }

        [TestMethod]
        public void NotSendEmailToAdmins_When_SixthUserWasSignedUp_And_UserIsAdmin()
        {
            InitializeTrainingManager();
            PrepareTrainingWith5Users();
            PrepareAdmins();

            manager.AddUserToTraining(EMAIL, TRAINING_ID, CURRENT_DATE, true);

            emailManager.Verify(r => r.SendSixthUserInTraining(admins, DATE_TIME), Times.Never);
        }
        private void PrepareAdmins()
        {
            admins = new List<IUser>();
            db.Setup(r => r.GetAdmins()).Returns(admins);
        }

        private void PrepareTrainingWith5Users()
        {
            var training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            training.SetupGet(r => r.RegisteredNumber).Returns(5);
            training.SetupGet(r => r.Capacity).Returns(10);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);
        }

        private void PrepareTrainingWithTwoUsers()
        {
            var usersInTrainingEmails = new List<string>() { EMAIL, EMAIL2 };
            db.Setup(r => r.GetEmailsOfAllUsersInTraining(TRAINING_ID)).Returns(usersInTrainingEmails);

            var training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training.Object);
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
        public void AddUserToTraining__But_EmailIsNotSent_When_UserIsNotAdmin()
        {
            InitializeTrainingManager();

            var message = manager.AddUserToTraining(EMAIL,TRAINING_ID,CURRENT_DATE);

            db.Verify(r => r.AddUserToTraining(EMAIL,TRAINING_ID,CURRENT_DATE), Times.Once);
            emailManager.Verify(r => r.SendAddingUserToTraining(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Never);
            Assert.AreEqual(CalendarMessages.SignUpSuccessful, message);
        }

        [TestMethod]
        public void AddUserToTraining__And_EmailIsSent_When_UserIsAdmin()
        {
            InitializeTrainingManager();
            training1.SetupGet(r => r.Time).Returns(DATE_TIME);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training1.Object);

            var message = manager.AddUserToTraining(EMAIL, TRAINING_ID, CURRENT_DATE,true);

            db.Verify(r => r.AddUserToTraining(EMAIL, TRAINING_ID, CURRENT_DATE), Times.Once);
            emailManager.Verify(r => r.SendAddingUserToTraining(EMAIL,DATE_TIME), Times.Once);
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
        public void AddTwoRecurringTrainingsWithMinDateIntoDB_When_TwoCheckedTimeslotsExist()
        {
            InitializeTrainingManager();
            PrepareModelWithTwoCheckedTimeslotsWithoutValidFrom();

            manager.CreateRecurringTraining(model);

            db.Verify(p => p.CreateRecurringTrainingTemplate(It.Is<RecurringTrainingTemplate>(r=>r.Trainer == TRAINER && r.Capacity == CAPACITY && r.Description == DESCRIPTION && r.Day == DAY_A && r.Time == TIME_A && r.ValidFrom == DateTime.MinValue)), Times.Once);
            db.Verify(p => p.CreateRecurringTrainingTemplate(It.Is<RecurringTrainingTemplate>(r=>r.Trainer == TRAINER && r.Capacity == CAPACITY && r.Description == DESCRIPTION && r.Day == DAY_B && r.Time == TIME_B && r.ValidFrom == DateTime.MinValue)), Times.Once);
        }

        [TestMethod]
        public void ClearLastTemplateGenerationFlag_When_TemplateWasAdded()
        {
            InitializeTrainingManager();
            PrepareModelWithTwoCheckedTimeslotsWithoutValidFrom();

            manager.CreateRecurringTraining(model);

            db.Verify(r => r.SetLastTemplateGenerationDate(CURRENT_DATE.AddDays(-1)));
        }

        [TestMethod]
        public void ClearLastTemplateGenerationFlag_When_TemplateWasRemoved()
        {
            InitializeTrainingManager();

            manager.RemoveTrainingTemplate(DAY_A, TIME_A);

            db.Verify(r => r.SetLastTemplateGenerationDate(CURRENT_DATE.AddDays(-1)), Times.Once);
        }

        [TestMethod]
        public void AddTwoRecurringTrainingsWithCorrectDateIntoDB_When_TwoCheckedTimeslotsExist()
        {
            InitializeTrainingManager();
            PrepareModelWithTwoCheckedTimeslotsWithValidFrom();

            manager.CreateRecurringTraining(model);

            db.Verify(p => p.CreateRecurringTrainingTemplate(It.Is<RecurringTrainingTemplate>(r => r.Trainer == TRAINER && r.Capacity == CAPACITY && r.Description == DESCRIPTION && r.Day == DAY_A && r.Time == TIME_A && r.ValidFrom == new DateTime(2017,03,15))), Times.Once);
            db.Verify(p => p.CreateRecurringTrainingTemplate(It.Is<RecurringTrainingTemplate>(r => r.Trainer == TRAINER && r.Capacity == CAPACITY && r.Description == DESCRIPTION && r.Day == DAY_B && r.Time == TIME_B && r.ValidFrom == new DateTime(2017, 03, 15))), Times.Once);
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

        [TestMethod]
        public void GenerateTrainingFromTemplate()
        {
            InitializeTrainingManager();
            PrepareTemplate();
            PrepareDBAndFactoryForGeneration();

           var model = manager.GenerateTrainingFromTemplate(template.Object, DATE_TIME);

            Assert.AreEqual(trainingModel.Object, model);
            var expectedDate = new DateTime(DATE_TIME.Year, DATE_TIME.Month, DATE_TIME.Day, TIME_A, 00, 00);
            db.Verify(r => r.CreateNewTraining(TRAINING_ID, expectedDate, TRAINER, DESCRIPTION, CAPACITY));
        }

        [TestMethod]
        public void ConfirmParticipation()
        {
            InitializeTrainingManager();

            manager.ConfirmParticipation(TRAINING_ID, EMAIL);

            db.Verify(r => r.ConfirmParticipation(TRAINING_ID, EMAIL), Times.Once);
        }

        [TestMethod]
        public void DisproveParticipation()
        {
            InitializeTrainingManager();

            manager.DisproveParticipation(TRAINING_ID, EMAIL);

            db.Verify(r => r.DisproveParticipation(TRAINING_ID, EMAIL), Times.Once);
        }

        [TestMethod]
        public void SignOutFromAllTrainings_When_DisproveParticipationIsExecuted()
        {
            InitializeTrainingManager();

            manager.DisproveParticipation(TRAINING_ID, EMAIL);

            db.Verify(r => r.SignOutUserFromAllTrainingsAfterDate(EMAIL,CURRENT_DATE), Times.Once);
        }

        [TestMethod]
        public void ForbidSigningUpToTrainings_When_DisproveParticipationIsExecuted()
        {
            InitializeTrainingManager();

            manager.DisproveParticipation(TRAINING_ID, EMAIL);

            db.Verify(r => r.ForbidSigningUpToTrainings(EMAIL), Times.Once);
        }

        [TestMethod]
        public void AllowSigningUpToTrainings_When_ConfirmParticipationIsExecuted()
        {
            InitializeTrainingManager();

            manager.ConfirmParticipation(TRAINING_ID, EMAIL);

            db.Verify(r => r.AllowSigningUpToTrainings(EMAIL), Times.Once);
        }

        private void PrepareDBAndFactoryForGeneration()
        {
            training1 = new Mock<ITraining>();
            trainingModel = new Mock<ITrainingModel>();
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training1.Object);
            factory.Setup(r => r.CreateTrainingModel(training1.Object)).Returns(trainingModel.Object);
            uidService.Setup(r => r.CreateID()).Returns(TRAINING_ID);
        }

        private void PrepareTemplate()
        {
            template = new Mock<IRecurringTrainingTemplate>();
            template.SetupGet(r => r.Capacity).Returns(CAPACITY);
            template.SetupGet(r => r.Description).Returns(DESCRIPTION);
            template.SetupGet(r => r.Trainer).Returns(TRAINER);
            template.SetupGet(r => r.Time).Returns(TIME_A);
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

        private void PrepareModelWithTwoCheckedTimeslotsWithoutValidFrom()
        {
            PrepareModelWithNoCheckedTimeslot();
            model.IsTrainingInTime[DAY_A*13 + TIME_A - 7] = true;
            model.IsTrainingInTime[DAY_B*13 + TIME_B - 7] = true;
        }

        private void PrepareModelWithTwoCheckedTimeslotsWithValidFrom()
        {
            PrepareModelWithNoCheckedTimeslot();
            model.ValidFrom = "15.03.2017";
            model.IsTrainingInTime[DAY_A * 13 + TIME_A - 7] = true;
            model.IsTrainingInTime[DAY_B * 13 + TIME_B - 7] = true;
        }


        private void PrepareModelWithNoCheckedTimeslot()
        {
            model = new RecurringModel();
            model.Capacity = CAPACITY;
            model.Description = DESCRIPTION;
            model.Trainer = TRAINER;
            model.ValidFrom = string.Empty;
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
            training3Model = new Mock<ITrainingForAdminModel>();
            training1Model.SetupGet(r => r.Time).Returns(DATE_IN_FUTURE);
            training2Model.SetupGet(r => r.Time).Returns(DATE_IN_FUTURE);
            training3Model.SetupGet(r => r.Time).Returns(DATE_IN_FUTURE);
            factory.Setup(r => r.CreateTrainingForAdminModel(training1.Object)).Returns(training1Model.Object);
            factory.Setup(r => r.CreateTrainingForAdminModel(training2.Object)).Returns(training2Model.Object);
            if(training3 != null)
            {
            factory.Setup(r => r.CreateTrainingForAdminModel(training3.Object)).Returns(training3Model.Object);
            }
        }

        private void PrepareDBWithTwoFutureTrainings()
        {
            training1 = new Mock<ITraining>();
            training2 = new Mock<ITraining>();
            training1.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            training2.Setup(r => r.Time).Returns(DATE_IN_FUTURE);
            var trainingsFromDB = new List<ITraining>() { training1.Object,training2.Object};

            db.Setup(r => r.GetAllTrainings()).Returns(trainingsFromDB);
        }
        private void PrepareDBWithTwoPastTrainings()
        {
            training1 = new Mock<ITraining>();
            training2 = new Mock<ITraining>();
            training1.Setup(r => r.Time).Returns(DATE_IN_PAST);
            training2.Setup(r => r.Time).Returns(DATE_IN_PAST);
            var trainingsFromDB = new List<ITraining>() { training1.Object, training2.Object };

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
            emailManager = new Mock<IEmailManager>();
            dateTimeService = new Mock<IDateTimeService>();
            dateTimeService.Setup(r => r.GetCurrentDate()).Returns(CURRENT_DATE);
            userManager = new Mock<IUserManager>();
            manager = new TrainingsManager(db.Object,factory.Object,uidService.Object,dateTimeService.Object,userManager.Object,emailManager.Object);

            userManager.Setup(r => r.UserExists(EMAIL)).Returns(true);
            userManager.Setup(r => r.UserExists(FALSE_EMAIL)).Returns(false);
            training1 = new Mock<ITraining>();
            training1.SetupGet(r => r.Capacity).Returns(10);
            db.Setup(r => r.GetTrainingData(TRAINING_ID)).Returns(training1.Object);
            db.Setup(r => r.GetTrainingData(FALSE_TRAINING_ID)).Returns((ITraining)null);
           
        }
    }
}
