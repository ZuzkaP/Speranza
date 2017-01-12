using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using System.Collections.Generic;
using System;

namespace Speranza.Services.Interfaces
{
    public interface ITrainingsManager
    {
        IList<ITrainingForAdminModel> GetAllTrainingsForAdmin();
        ITrainingModel RemoveUserFromTraining(string email, string id);
        void SetTrainer(string traininingID, string trainer);
        IList<IUserForTrainingDetailModel> GetAllUsersInTraining(string trainingID);
        void SetTrainingDescription(string trainingID, string trainingDescription);
        void SetTrainingCapacity(string trainingID, int capacity);
        string CreateNewTraining(DateTime dateTime, string trainer, string description, int capacity);
        void CancelTraining(string trainingID);
    }
}