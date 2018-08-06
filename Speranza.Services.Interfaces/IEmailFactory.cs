using System;
using System.Collections.Generic;
using Speranza.Database.Data.Interfaces;
using Speranza.Smtp.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface IEmailFactory
    {
        Email CreateWelcomeEmail(string email, string welcomeSubject, string welcomeBody);
        Email CreateTrainingCanceledEmail(string email, string trainingCanceledSubject, string trainingCanceledBody, DateTime dateTime);
        Email CreateAddingUserToTrainingEmail(string email, string addingUserToTrainingSubject, string addingUserToTrainingBody, DateTime dateTime);
        Email CreateRemovingUserFromTrainingEmail(string email, string removingUserFromTrainingSubject, string removingUserFromTrainingBody, DateTime dateTime);
        Email CreateConfirmAttendanceEmail(IList<IUser> admins, IList<ITraining> trainings, string confirmAttendanceSubject, string confirmAttendanceBody);
        Email Create6thUserSignepUpEmail(IList<IUser> admins, string sixthUserSignedUpInTrainingSubject, string sixthUserSignedUpInTrainingBody, DateTime dateTime);
        Email Create6thUserSignOffEmail(IList<IUser> admins, string sixthUserSignedOffFromTrainingSubject, string sixthUserSignedOffFromTrainingBody, DateTime dateTime);
        Email CreatePassRecoveryEmail(string email, string recoveryPassSubject, string recoveryPassBody, string newPass);
    }
}