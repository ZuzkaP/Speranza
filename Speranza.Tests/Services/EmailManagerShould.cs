using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Moq;
using Speranza.Smtp.Interfaces;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class EmailManagerShould
    {
        private const string EMAIL = "email";
        private EmailManager emailManager;
        private Mock<ISmtp> smtp;
        private Email email;
        private Mock<IEmailFactory> factory;
        private readonly DateTime DATE_TIME = new DateTime(2017, 08, 08, 10, 00, 00);

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
            emailManager = new EmailManager(factory.Object,smtp.Object);
        }
    }
}
