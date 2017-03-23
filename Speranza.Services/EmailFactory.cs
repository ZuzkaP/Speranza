using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Smtp.Interfaces;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Services
{
    public class EmailFactory : IEmailFactory
    {
        public Email CreateAddingUserToTrainingEmail(string email, string addingUserToTrainingSubject, string addingUserToTrainingBody, DateTime dateTime)
        {
            Email result = new Email();
            result.Receiver = email;
            result.Subject = string.Format(addingUserToTrainingSubject, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));
            result.Body = string.Format(addingUserToTrainingBody, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));

            return result;
        }

        public Email CreateConfirmAttendanceEmail(IList<IUser> admins, IList<IUser> users, string trainingId, DateTime dateTime, string confirmAttendanceSubject, string confirmAttendanceBody)
        {
            Email result = new Email();
            foreach (var item in admins)
            {
            result.Receiver += item.Email + ",";
            }
            result.Receiver = result.Receiver.TrimEnd(',');
            result.Subject = string.Format(confirmAttendanceSubject, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));


            return result;
        }

        public Email CreateRemovingUserFromTrainingEmail(string email, string removingUserFromTrainingSubject, string removingUserFromTrainingBody, DateTime dateTime)
        {
            Email result = new Email();
            result.Receiver = email;
            result.Subject = string.Format(removingUserFromTrainingSubject, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));
            result.Body = string.Format(removingUserFromTrainingBody, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));

            return result;
        }

        public Email CreateTrainingCanceledEmail(string email, string trainingCanceledSubject, string trainingCanceledBody, DateTime dateTime)
        {
            Email result = new Email();
            result.Receiver = email;
            result.Subject = string.Format(trainingCanceledSubject, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));
            result.Body = string.Format(trainingCanceledBody, dateTime.ToString("dd.MM.yyyy"), dateTime.ToString("HH:mm"));

            return result;
        }

        public Email CreateWelcomeEmail(string email, string welcomeSubject, string welcomeBody)
        {
            Email result = new Email()
            {
                Body = welcomeBody,
                Receiver = email,
                Subject = welcomeSubject
            };
            return result;
        }
    }
}
