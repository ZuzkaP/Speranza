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

        [TestMethod]
        public void SendWelcome()
        {
            InitializeEmailManager();
            PrepareWelcomeEmailMessage();

            emailManager.SendWelcome(EMAIL);

            smtp.Verify(r => r.SendEmail(email), Times.Once);
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
