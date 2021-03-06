﻿using Speranza.Models.Interfaces;
using System.Collections.Generic;
using System;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface ITrainingsManager
    {
        IList<ITrainingForAdminModel> GetFutureTrainings(int from, int to);
        IList<ITrainingForAdminModel> GetFutureTrainings(DateTime date);
        IList<ITrainingModel> GetFutureTrainings();
        ITrainingModel RemoveUserFromTraining(string email, string id);
        ITrainingModel RemoveUserFromTraining(string email, string id,bool isAdmin);
        void SetTrainer(string traininingID, string trainer);
        IList<IUserForTrainingDetailModel> GetAllUsersInTraining(string trainingID);
        void SetTrainingDescription(string trainingID, string trainingDescription);
        void SetTrainingCapacity(string trainingID, int capacity);
        string CreateNewTraining(DateTime dateTime, string trainer, string description, int capacity);
        void CancelTraining(string trainingID);
        void SetSignOffLimit(int hoursLimit);
        int GetSignOffLimit();
        CalendarMessages AddUserToTraining(string email, string trainingID, DateTime currentDate, bool isAdmin);
        CalendarMessages AddUserToTraining(string email, string trainingID, DateTime currentDate);
        void CreateRecurringTraining(IRecurringModel model);
        IList<IRecurringTemplateModel> GetTemplates();
        void RemoveTrainingTemplate(int day, int time);
        ITrainingModel GenerateTrainingFromTemplate(IRecurringTrainingTemplate template, DateTime date);
        int GetFutureTrainingsCount();
        IList<ITrainingForAdminModel> GetPastTrainings(int from, int to);
        IList<ITrainingForAdminModel> GetPastTrainings(DateTime date);
        int GetPastTrainingsCount();
        void ConfirmParticipation(string trainingID, string email);
        void DisproveParticipation(string trainingID, string email);
    }
}