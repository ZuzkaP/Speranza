using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database;

namespace Speranza.Smtp
{
    public class Smtp : ISmtp
    {

        private IDatabaseGateway db;
        private const int PORT = 25;
        private const string HOST = "HOST";
        private const string SENDER = "speranza@test.com";
        
        public Smtp(IDatabaseGateway db)
        {
            this.db = db;
        }
        
        public void SendEmail(Email email)
        {
            SmtpClient client = new SmtpClient(HOST, PORT);
            try
            {
                client.SendAsync(SENDER, email.Receiver, email.Subject, email.Body, null);
            }
            catch (Exception e)
            {
                db.WriteToLog(e.Message,email);
            }
            client.Dispose();
        }
    }
}
