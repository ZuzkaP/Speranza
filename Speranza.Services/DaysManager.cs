using System;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Database;

namespace Speranza.Services
{
    public class DaysManager : IDaysManager
    {
        private IDateTimeService dateTimeService;
        IDatabaseGateway db;
        private ITrainingsManager manager;

        public DaysManager(IDatabaseGateway db,ITrainingsManager manager, IDateTimeService dateTimeService)
        {
            this.db = db;
            this.manager = manager;
            this.dateTimeService = dateTimeService;
        }

        public IDayModel GetDay(DateTime date)
        {
            IDayModel model = new DayModel(date.ToString("d.MM."),dateTimeService.GetDayName(date));
            var trainings = db.GetDayTrainings(date);

            if(trainings != null)
            {
                foreach (var item in trainings)
                {
                    model.Trainings.Add(manager.CreateModel(item));
                }
            }
            return model;
        }
    }
}