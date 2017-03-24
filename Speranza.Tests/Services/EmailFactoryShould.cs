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
        private Mock<IUser> user;
        private const string BODY_FOR_ATTENDANCE = "uvod {0} zaver";
        private const string SUBBODY_FOR_ATTENDANCE = "{0} {1} {2} {3}";

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
        public void CreateRightReceiver_When_OnlyOneExists_AndCreateConfirmAttendanceEmailIsCalled()
        {
            InitializeEmailFactory();
            PrepareDataForConfirmAttendanceWithOneAdmin();
            PrepareOneUserToConfirmHisAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, users, TRAINING_ID, DATE_TIME, SUBJECT, BODY,SUBBODY_FOR_ATTENDANCE);

            Assert.AreEqual(EMAIL, result.Receiver);
        }

        [TestMethod]
        public void CreateRightReceiver_When_TwoAdminExist_AndCreateConfirmAttendanceEmailIsCalled()
        {
            InitializeEmailFactory();
            PrepareDataForConfirmAttendanceWithTwoAdmins();
            PrepareOneUserToConfirmHisAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, users, TRAINING_ID, DATE_TIME, SUBJECT, BODY,SUBBODY_FOR_ATTENDANCE);

            Assert.AreEqual(EMAIL+"," + EMAIL2, result.Receiver);
        }

        [TestMethod]
        public void CreateConfirmAttendanceEmailSubject()
        {
            InitializeEmailFactory();
            PrepareDataForConfirmAttendanceWithOneAdmin();
            PrepareOneUserToConfirmHisAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, users, TRAINING_ID, DATE_TIME, SUBJECT_2_PARAMS, BODY,SUBBODY_FOR_ATTENDANCE);

            Assert.AreEqual(string.Format(SUBJECT_2_PARAMS, DATE_TIME.ToString("dd.MM.yyyy"), DATE_TIME.ToString("HH:mm")), result.Subject);
        }

        [TestMethod]
        public void CreateConfirmAttendanceEmailBodyForOneUser()
        {
            InitializeEmailFactory();
            PrepareDataForConfirmAttendanceWithOneAdmin();
            PrepareOneUserToConfirmHisAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, users, TRAINING_ID, DATE_TIME, SUBJECT_2_PARAMS, BODY_FOR_ATTENDANCE,SUBBODY_FOR_ATTENDANCE);

            Assert.AreEqual("uvod " + NAME + " " + SURNAME + " " + EMAIL + " " + TRAINING_ID + " zaver",result.Body);
        }
        [TestMethod]
        public void CreateConfirmAttendanceEmailBodyForTwoUsers()
        {
            InitializeEmailFactory();
            PrepareDataForConfirmAttendanceWithOneAdmin();
            PrepareTwoUsersToConfirmHisAttendance();

            Email result = factory.CreateConfirmAttendanceEmail(admins, users, TRAINING_ID, DATE_TIME, SUBJECT_2_PARAMS, BODY_FOR_ATTENDANCE, SUBBODY_FOR_ATTENDANCE);

            Assert.AreEqual("uvod " + NAME + " " + SURNAME + " " + EMAIL + " " + TRAINING_ID + NAME2 + " " + SURNAME2 + " " + EMAIL2 + " " + TRAINING_ID + " zaver", result.Body);
        }

        private void PrepareTwoUsersToConfirmHisAttendance()
        {
            PrepareOneUserToConfirmHisAttendance();
            var user = new Mock<IUser>();
            user.SetupGet(r => r.Email).Returns(EMAIL2);
            user.SetupGet(r => r.Name).Returns(NAME2);
            user.SetupGet(r => r.Surname).Returns(SURNAME2);
            users.Add(user.Object);
        }

        private void PrepareOneUserToConfirmHisAttendance()
        {
            user = new Mock<IUser>();
            user.SetupGet(r => r.Email).Returns(EMAIL);
            user.SetupGet(r => r.Name).Returns(NAME);
            user.SetupGet(r => r.Surname).Returns(SURNAME);
            users = new List<IUser>();
            users.Add(user.Object);
        }

       

        private void PrepareDataForConfirmAttendanceWithTwoAdmins()
        {
            var admin = new Mock<IUser>();
            var admin2 = new Mock<IUser>();
            admin.SetupGet(r => r.Email).Returns(EMAIL);
            admin2.SetupGet(r => r.Email).Returns(EMAIL2);
            admins = new List<IUser>() { admin.Object,admin2.Object };

        }

        private void PrepareDataForConfirmAttendanceWithOneAdmin()
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
