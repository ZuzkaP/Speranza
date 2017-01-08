using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using System.Collections.Generic;

namespace Speranza.Services.Interfaces
{
    public interface ITrainingsManager
    {
        IList<ITrainingForAdminModel> GetAllTrainingsForAdmin();
        ITrainingModel RemoveUserFromTraining(string email, string id);
        void SetTrainer(string traininingID, string trainer);
        IList<IUserForTrainingDetailModel> GetAllUsersInTraining(string trainingID);
    }
}