using Speranza.Database.Data.Interfaces;
using Speranza.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using Speranza.Database.Data;
using Speranza.Common.Data;

namespace Speranza.Database
{
    public interface IDatabaseGateway
    {
        void RegisterNewUser(string email, string name,string password, string phoneNumber, string surname);
        bool UserExists(string email);
        IUser LoadUser(string email);
        IList<ITraining> GetDayTrainings(DateTime date);
        IUser GetUserData(string email);
        void UpdateUserData(string email, string name, string surname, string phoneNumber);
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
        IList<IUser> GetUsersInTraining(string trainingID);
        void SetTrainingDescription(string trainingID, string trainingDescription);
        void SetTrainingCapacity(string trainingID, int capacity);
        void CreateNewTraining(string trainingID, DateTime dateTime, string trainer, string trainingDescription, int capacity);
        void CancelTraining(string trainingID);
        void SetSignOffLimit(int hoursLimit);
        int GetSignOffLimit();
        void ChangePassword(string email, string newpasswordhash);
        void CreateRecurringTrainingTemplate(RecurringTrainingTemplate recurringTrainingTemplate);
        IList<IRecurringTrainingTemplate> GetTemplates();
        void RemoveTrainingTemplate(int day, int time);
        IList<IRecurringTrainingTemplate> GetTemplatesForTheDay(int day);
        void SetLastTemplateGenerationDate(DateTime dateTime);
        DateTime GetLastTemplateGenerationDate();
        int GetTrainingsCountAfterDate(DateTime date);
        int GetTrainingsCountBeforeDate(DateTime date);
    }
}