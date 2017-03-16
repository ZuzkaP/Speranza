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

        private void InitializeEmailFactory()
        {
            factory = new EmailFactory();
        }
    }
}
