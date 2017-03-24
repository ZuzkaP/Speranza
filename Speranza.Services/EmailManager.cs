using Speranza.Services.Interfaces;
using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database.Data.Interfaces;

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

        public void SendAddingUserToTraining(string receiver, DateTime dateTime)
        {
            Email email = factory.CreateAddingUserToTrainingEmail(receiver, EmailMessages.AddingUserToTrainingSubject, EmailMessages.AddingUserToTrainingBody, dateTime);
            smtp.SendEmail(email);
        }

        public void SendConfirmUserAttendance(IList<IUser> admins, IList<IUser> users, string trainingID,DateTime dateTime)
        {
            Email email = factory.CreateConfirmAttendanceEmail(admins,users,trainingID,dateTime,EmailMessages.ConfirmAttendanceSubject, EmailMessages.ConfirmAttendanceBody,string.Empty);
            smtp.SendEmail(email);
        }

        public void SendRemovingUserFromTraining(string receiver, DateTime dateTime)
        {
            Email email = factory.CreateRemovingUserFromTrainingEmail(receiver, EmailMessages.RemovingUserFromTrainingSubject, EmailMessages.RemovingUserFromTrainingBody, dateTime);
            smtp.SendEmail(email);
        }

        public void SendSixthUserInTraining(IList<IUser> admins, DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void SendTrainingCanceled(string receiver, DateTime dateTime)
        {
            Email email = factory.CreateTrainingCanceledEmail(receiver, EmailMessages.TrainingCanceledSubject, EmailMessages.TrainingCanceledBody,dateTime);
            smtp.SendEmail(email);
        }

        public void SendWelcome(string receiver)
        {
            Email email = factory.CreateWelcomeEmail(receiver, EmailMessages.WelcomeSubject, EmailMessages.WelcomeBody);
            smtp.SendEmail(email);
        }
    }
}
