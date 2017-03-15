using Speranza.Services.Interfaces;
using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services
{
    public class EmailManager : IEmailManager
    {
        private IEmailFactory factory;
        private ISmtp smtp;

        public EmailManager(IEmailFactory factory, ISmtp smtp)
        {
            this.factory = factory;
            this.smtp = smtp;
        }

        public void SendWelcome(string receiver)
        {
            Email email = factory.CreateWelcomeEmail(receiver, EmailMessages.WelcomeSubject, EmailMessages.WelcomeBody);
            smtp.SendEmail(email);
        }
    }
}
