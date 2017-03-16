using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private const string CONTENT = "content";
        private const string SUBJECT_2_PARAMS = "subject{0}{1}";
        private const string BODY_2_PARAMS = "content{0}{1}";
        private readonly DateTime DATE_TIME = new DateTime(2017, 08, 08, 10, 00, 00);
        private EmailFactory factory;

        [TestMethod]
        public void CreateWelcomeEmail()
        {
            InitializeEmailFactory();

            Email result = factory.CreateWelcomeEmail(EMAIL, SUBJECT, CONTENT);

            Assert.AreEqual(CONTENT, result.Body);
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


        private void InitializeEmailFactory()
        {
            factory = new EmailFactory();
        }
    }
}
