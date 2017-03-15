using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Smtp
{
    public class Smtp : ISmtp

    {
        private const int PORT = 25;
        private const string HOST = "HOST";
        private const string SENDER = "speranza@test.com";

        public void SendEmail(Email email)
        {
            SmtpClient client = new SmtpClient(HOST, PORT);
            client.SendAsync(SENDER, email.Receiver, email.Subject, email.Content, null);
            client.Dispose();
        }
    }
}
