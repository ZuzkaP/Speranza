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

        public TrainingsManager(IDatabaseGateway db,IModelFactory factory)
        {
            this.db = db;
            this.factory = factory;
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
    }
}