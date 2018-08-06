using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Database.Data.Interfaces;
using Speranza.Services;
using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class EmailFactoryShould
    {
        private const string EMAIL = "email";
        private const string SUBJECT = "subject";
        private const string BODY = "content";
        private const string SUBJECT_2_PARAMS = "subject{0}{1}";
        private const string BODY_2_PARAMS = "content{0}{1}";
        private const string TRAINING_ID = "trainingId";
        private const string EMAIL2 = "EMAIL2";
        private const string NAME = "meno";
        private const string NAME2 = "meno2";
        private const string SURNAME = "priezvisko";
        private const string SURNAME2 = "priezvisko2";
        private readonly DateTime DATE_TIME = new DateTime(2017, 08, 08, 10, 00, 00);
        private EmailFactory factory;
        private IList<IUser> admins;
        private List<IUser> users;
        private List<ITraining> trainings;
        private Mock<IUser> user;
        private Mock<ITraining> training;
        private const string BODY_FOR_ATTENDANCE = "uvod {0} zaver";
        private const string SUBBODY_FOR_ATTENDANCE = "{0} {1} {2} {3}";
        private const string BODY_1_PARAM = "1PARAM_BODY {0}";
        private const string SUBJECT_1_PARAM = "1PARAM_SUBJECT {0}";
        private const string NEW_PASS = "NEWPASS";

        [TestMethod]
        public void CreateWelcomeEmail()
        {
            InitializeEmailFactory();

            Email result = factory.CreateWelcomeEmail(EMAIL, SUBJECT, BODY);

            Assert.AreEqual(BODY, result.Body);
            Assert.AreEqual(EMAIL, result.Receiver);
            Assert.AreEqual(SUBJECT, result.Subject);

        }

        [TestMethod]
        public void CreateCanceledTrainingEmail()
        {
            InitializeEmailFactory();

            Email result = factory.CreateTrainingCanceledEmail(EMAIL, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);
            
            Assert.AreEqual(EMAIL, result.Receiver);
           
            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS,DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS,DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }

        [TestMethod]
        public void CreateAddingUserToTrainingEmail()
        {
            InitializeEmailFactory();

            Email result = factory.CreateAddingUserToTrainingEmail(EMAIL, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);

            Assert.AreEqual(EMAIL, result.Receiver);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }

        [TestMethod]
        public void CreateRemovingUserFromTrainingEmail()
        {
            InitializeEmailFactory();

            Email result = factory.CreateRemovingUserFromTrainingEmail(EMAIL, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);

            Assert.AreEqual(EMAIL, result.Receiver);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }

        [TestMethod]
        public void CreateSixthUserSignedUpInTrainingEmailWithOneAdmin()
        {
            InitializeEmailFactory();
            PrepareOneAdmin();

            Email result = factory.Create6thUserSignepUpEmail(admins, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);

            Assert.AreEqual(EMAIL, result.Receiver);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }
        [TestMethod]
        public void CreateSixthUserSignedUpInTrainingEmailWithTwoAdmins()
        {
            InitializeEmailFactory();
            PrepareTwoAdmins();

            Email result = factory.Create6thUserSignepUpEmail(admins, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);

            Assert.AreEqual(EMAIL + "," + EMAIL2, result.Receiver);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }

        [TestMethod]
        public void CreateSixthUserSignedOffFromTrainingEmailWithOneAdmin()
        {
            InitializeEmailFactory();
            PrepareOneAdmin();

            Email result = factory.Create6thUserSignOffEmail(admins, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);

            Assert.AreEqual(EMAIL, result.Receiver);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }
        [TestMethod]
        public void CreateSixthUserSignedOffFromTrainingEmailWithTwoAdmins()
        {
            InitializeEmailFactory();
            PrepareTwoAdmins();

            Email result = factory.Create6thUserSignOffEmail(admins, SUBJECT_2_PARAMS, BODY_2_PARAMS, DATE_TIME);

            Assert.AreEqual(EMAIL + "," + EMAIL2, result.Receiver);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
            Assert.AreEqual(string.Format(BODY_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Body);
        }

        [TestMethod]
        public void CreateRightReceiver_When_OnlyOneExists_AndCreateConfirmAttendanceEmailIsCalled()
        {
            InitializeEmailFactory();
            PrepareOneAdmin();
            PrepareTrainingsToConfirmAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, trainings, SUBJECT, BODY);

            Assert.AreEqual(EMAIL, result.Receiver);
        }

        [TestMethod]
        public void CreateRightReceiver_When_TwoAdminExist_AndCreateConfirmAttendanceEmailIsCalled()
        {
            InitializeEmailFactory();
            PrepareTwoAdmins();
            PrepareTrainingsToConfirmAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, trainings, SUBJECT, BODY);

            Assert.AreEqual(EMAIL+"," + EMAIL2, result.Receiver);
        }

        [TestMethod]
        public void CreateConfirmAttendanceEmailSubject()
        {
            InitializeEmailFactory();
            PrepareOneAdmin();
            PrepareTrainingsToConfirmAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, trainings, SUBJECT_1_PARAM, BODY_1_PARAM);

            Assert.AreEqual("1PARAM_SUBJECT 08.08.2017", result.Subject);
        }

        [TestMethod]
        public void CreateConfirmAttendanceEmailBodyForOneUser()
        {
            InitializeEmailFactory();
            PrepareOneAdmin();
            PrepareTrainingsToConfirmAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, trainings, SUBJECT_1_PARAM, BODY_1_PARAM);

            Assert.AreEqual("1PARAM_BODY 08.08.2017 10:00", result.Body.Trim());
        }
        [TestMethod]
        public void CreateConfirmAttendanceEmailBodyForTwoUsers()
        {
            InitializeEmailFactory();
            PrepareOneAdmin();
            PrepareTrainingsToConfirmAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, trainings, SUBJECT, BODY_1_PARAM);

            Assert.AreEqual("1PARAM_BODY 08.08.2017 10:00", result.Body.Trim());
        }

        [TestMethod]
        public void CreatePassRecoveryEmail()
        {
            InitializeEmailFactory();

            Email result = factory.CreatePassRecoveryEmail(EMAIL, SUBJECT, BODY_1_PARAM, NEW_PASS);

            Assert.AreEqual(EMAIL, result.Receiver);

            Assert.AreEqual(SUBJECT, result.Subject);
            Assert.AreEqual(string.Format(BODY_1_PARAM, NEW_PASS), result.Body);
        }
        

        private void PrepareTrainingsToConfirmAttendance()
        {
            training = new Mock<ITraining>();
            training.SetupGet(r => r.Time).Returns(DATE_TIME);
            trainings = new List<ITraining>();
            trainings.Add(training.Object);
        }
        
        private void PrepareTwoAdmins()
        {
            var admin = new Mock<IUser>();
            var admin2 = new Mock<IUser>();
            admin.SetupGet(r => r.Email).Returns(EMAIL);
            admin2.SetupGet(r => r.Email).Returns(EMAIL2);
            admins = new List<IUser>() { admin.Object,admin2.Object };

        }

        private void PrepareOneAdmin()
        {
            var admin = new Mock<IUser>();
            admin.SetupGet(r => r.Email).Returns(EMAIL);
            admins = new List<IUser>() { admin.Object};

        }

        private void InitializeEmailFactory()
        {
            factory = new EmailFactory();
        }
    }
}
