using System;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Database;

namespace Speranza.Services
{
    public class DaysManager : IDaysManager
    {
        IDatabaseGateway db;
        private ITrainingsManager manager;

        public DaysManager(IDatabaseGateway db,ITrainingsManager manager)
        {
            this.db = db;
            this.manager = manager;
        }

        public IDayModel GetDay(DateTime date)
        {
            IDayModel model = new DayModel();
            var trainings = db.GetDayTrainings(date);
            foreach (var item in trainings)
            {
             model.Trainings.Add(manager.CreateModel(item));
            }
            return model;
        }
    }
}