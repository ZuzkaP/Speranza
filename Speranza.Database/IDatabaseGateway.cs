﻿using Speranza.Database.Data.Interfaces;
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
        ITraining GetTrainingData(string trainingID);
        void AddUserToTraining(string email, string trainingID,DateTime timeOfSignUp);
        bool IsUserAlreadySignedUpInTraining(string email, string trainingID);
        void RemoveUserFromTraining(string email, string trainingID);
        IList<ITraining> GetTrainingsForUser(string email);
        IList<IUser> GetAllUsers();
        IList<ITraining> GetAllTrainings();
        void SetAdminRole(string email, bool isAdmin);
        void SetUserCategory(string email, UserCategories category);
        int UpdateCountOfFreeSignUps(string email, int changeNumberOfSignUps);
        void SetTrainer(string trainingID, string trainer);
    }
}