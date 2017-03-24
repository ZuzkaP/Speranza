using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Moq;
using Speranza.Smtp.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Database.Data.Interfaces;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class EmailManagerShould
    {
        private const string EMAIL = "email";
        private const string TRAINING_ID = "traininigID";
        private const string EMAIL2 = "EMAIL2";
        private EmailManager emailManager;
        private Mock<ISmtp> smtp;
        private Email email;
        private Mock<IEmailFactory> factory;
        private readonly DateTime DATE_TIME = new DateTime(2017, 08, 08, 10, 00, 00);
        private Mock<IList<IUser>> users;
        private Mock<IList<IUser>> admins;

        [TestMethod]
        public void SendWelcome()
        {
            InitializeEmailManager();
            PrepareWelcomeEmailMessage();

            emailManager.SendWelcome(EMAIL);

            smtp.Verify(r => r.SendEmail(email), Times.Once);
        }

        [TestMethod]
        public void SendTrainingCanceled()
        {
            InitializeEmailManager();
            PrepareCancelEmailMessage();

            emailManager.SendTrainingCanceled(EMAIL, DATE_TIME);

            smtp.Verify(r => r.SendEmail(email), Times.Once);
        }


        [TestMethod]
        public void SendAddingUserToTraining()
        {
            InitializeEmailManager();
            PrepareAddingUserToTrainingEmailMessage();

            emailManager.SendAddingUserToTraining(EMAIL, DATE_TIME);

            smtp.Verify(r => r.SendEmail(email), Times.Once);
        }

        [TestMethod]
        public void SendRemovingUserFromTraining()
        {
            InitializeEmailManager();
            PrepareRemovingUserFromTrainingEmailMessage();

            emailManager.SendRemovingUserFromTraining(EMAIL, DATE_TIME);

            smtp.Verify(r => r.SendEmail(email), Times.Once);
        }


        [TestMethod]
        public void SendConfirmUserAttendance()
        {
            InitializeEmailManager();
            PrepareConfirmAttendanceEmailMessage();

            emailManager.SendConfirmUserAttendance(admins.Object, users.Object, TRAINING_ID, DATE_TIME);

            smtp.Verify(r => r.SendEmail(email), Times.Once);
        }

        private void PrepareConfirmAttendanceEmailMessage()
        {
            EmailMessages.ConfirmAttendanceSubBody = string.Empty;
            users = new Mock<IList<IUser>>();
            admins = new Mock<IList<IUser>>();
            email = new Email();
            factory.Setup(r => r.CreateConfirmAttendanceEmail(admins.Object, users.Object, TRAINING_ID, DATE_TIME, EmailMessages.ConfirmAttendanceSubject, EmailMessages.ConfirmAttendanceBody, EmailMessages.ConfirmAttendanceSubBody)).Returns(email);
        }

        private void PrepareRemovingUserFromTrainingEmailMessage()
        {
            email = new Email();
            factory.Setup(r => r.CreateRemovingUserFromTrainingEmail(EMAIL, EmailMessages.RemovingUserFromTrainingSubject, EmailMessages.RemovingUserFromTrainingBody, DATE_TIME)).Returns(email);
        }

        private void PrepareAddingUserToTrainingEmailMessage()
        {
            email = new Email();
            factory.Setup(r => r.CreateAddingUserToTrainingEmail(EMAIL, EmailMessages.AddingUserToTrainingSubject, EmailMessages.AddingUserToTrainingBody, DATE_TIME)).Returns(email);
        }

        private void PrepareCancelEmailMessage()
        {
            email = new Email();
            factory.Setup(r => r.CreateTrainingCanceledEmail(EMAIL, EmailMessages.TrainingCanceledSubject, EmailMessages.TrainingCanceledBody, DATE_TIME)).Returns(email);
        }

        private void PrepareWelcomeEmailMessage()
        {
            email = new Email();
            factory.Setup(r => r.CreateWelcomeEmail(EMAIL, EmailMessages.WelcomeSubject, EmailMessages.WelcomeBody)).Returns(email);
        }

        private void InitializeEmailManager()
        {
            factory = new Mock<IEmailFactory>();
            smtp = new Mock<ISmtp>();
            emailManager = new EmailManager(factory.Object, smtp.Object);
        }
    }
}
