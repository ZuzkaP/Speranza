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

        public DaysManager(IDatabaseGateway db, ITrainingsManager manager, IDateTimeService dateTimeService)
        {
            this.db = db;
            this.manager = manager;
            this.dateTimeService = dateTimeService;
        }

        public IDayModel GetDay(DateTime date, string email)
        {
            IDayModel model = new DayModel(date.ToString("d.MM."), dateTimeService.GetDayName(date));
            var trainings = db.GetDayTrainings(date);

            if (trainings != null)
            {
                foreach (var item in trainings)
                {
                    ITrainingModel trainingModel = manager.CreateModel(item);
                    trainingModel.IsUserSignedUp = db.IsUserAlreadySignedUpInTraining(email, item.ID);
                    DateTime currentDate = dateTimeService.GetCurrentDate();
                    if (trainingModel.Time <= currentDate)
                    {
                        trainingModel.IsAllowedToSignedUp = false;
                    }
                    else
                    {
                        trainingModel.IsAllowedToSignedUp = true;
                    }
                    model.Trainings.Add(trainingModel);
                }
            }
            return model;
        }
    }
}