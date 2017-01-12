using System;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Models;
using System.Collections.Generic;
using Speranza.Database;

namespace Speranza.Services
{
    public class TrainingsManager: ITrainingsManager
    {
        private IDatabaseGateway db;
        private IModelFactory factory;
        private IUidService uidService;

        public TrainingsManager(IDatabaseGateway db,IModelFactory factory, IUidService uidService)
        {
            this.db = db;
            this.factory = factory;
            this.uidService = uidService;
        }

        public void CancelTraining(string trainingID)
        {
            throw new NotImplementedException();
        }

        public string CreateNewTraining(DateTime dateTime, string trainer, string description)
        {
            string trainingID = uidService.CreateID();
            db.CreateNewTraining(trainingID, dateTime, trainer, description);

            return trainingID;
        }

        public IList<ITrainingForAdminModel> GetAllTrainingsForAdmin()
        {
            var trainingsFromDB = db.GetAllTrainings();
            var trainingsForAdmin = new List<ITrainingForAdminModel>();
            foreach (var item in trainingsFromDB)
            {
               ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(item);
                trainingsForAdmin.Add(model);
            }
            return trainingsForAdmin;
        }

        public IList<IUserForTrainingDetailModel> GetAllUsersInTraining(string trainingID)
        {
           IList<IUser> users = db.GetUsersInTraining(trainingID);
            IList<IUserForTrainingDetailModel> models = new List<IUserForTrainingDetailModel>();
            foreach (var item in users)
            {
                IUserForTrainingDetailModel model = factory.CreateUsersForTrainingDetailModel(item);
                models.Add(model);
            }
            return models;
        }

        public ITrainingModel RemoveUserFromTraining(string email, string id)
        {
            db.RemoveUserFromTraining(email, id);
            ITraining training = db.GetTrainingData(id);
            ITrainingModel model = factory.CreateTrainingModel(training);

            return model;
        }

        public void SetTrainer(string trainingID, string trainer)
        {
            db.SetTrainer(trainingID, trainer);
        }

        public void SetTrainingCapacity(string trainingID, int capacity)
        {
            db.SetTrainingCapacity(trainingID, capacity);
        }

        public void SetTrainingDescription(string trainingID, string trainingDescription)
        {
            db.SetTrainingDescription(trainingID, trainingDescription);
        }
    }
}