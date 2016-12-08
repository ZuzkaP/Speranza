using Speranza.Database.Data.Interfaces;
using Speranza.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Speranza.Database
{
    public interface IDatabaseGateway
    {
        void RegisterNewUser(RegisterModel model);
        bool UserExists(string email);
        IUser LoadUser(string email);
        IList<ITraining> GetDayTrainings(DateTime date);
        IUser GetUserData(string email);
        void UpdateUserData(UserProfileModel userProfileModel);
        ITraining GetTrainingData(string id);
        void AddUserToTraining(string email, string trainingID);
    }
}