using System;
using System.Collections.Generic;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface IEmailManager
    {
        void SendWelcome(string email);
        void SendTrainingCanceled(string email, DateTime dateTime);
        void SendAddingUserToTraining(string email, DateTime dateTime);
        void SendRemovingUserFromTraining(string email, DateTime dateTime);
        void SendConfirmUserAttendance(IList<IUser> admins, IList<IUser> users, DateTime dateTime);
    }
}