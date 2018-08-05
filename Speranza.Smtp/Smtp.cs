using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        private const string HOST = "smtp.forpsi.com";
        private const string SENDER = "treningy@speranza.sk";
        
        public Smtp(IDatabaseGateway db)
        {
            this.db = db;
        }
        
        public void SendEmail(Email email)
        {
            SmtpClient client = new SmtpClient(HOST);
            try
            {
                client.Credentials = new NetworkCredential("postmaster@speranza.sk", "d2P5Tkh_66");
                client.Send(SENDER, email.Receiver, email.Subject, email.Body);
            }
            catch (SmtpException ex)
            {
                var text = ex.Message +" - " + ex.StatusCode;
                if (ex.InnerException != null)
                {
                    text += " " + ex.InnerException.Message;
                }
                    db.WriteToLog(text, email);
            }
            catch (Exception e)
            {
                db.WriteToLog(e.Message,email);
            }
            client.Dispose();
        }
    }
}
