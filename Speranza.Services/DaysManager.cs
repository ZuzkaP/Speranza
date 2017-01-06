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
        private IModelFactory factory;
        private ITrainingsManager manager;

        public DaysManager(IDatabaseGateway db, ITrainingsManager manager, IDateTimeService dateTimeService, IModelFactory factory)
        {
            this.db = db;
            this.manager = manager;
            this.dateTimeService = dateTimeService;
            this.factory = factory;
        }

        public IDayModel GetDay(DateTime date, string email)
        {
            IDayModel model = new DayModel(date.ToString("d.M."), dateTimeService.GetDayName(date));
            var trainings = db.GetDayTrainings(date);

            if (trainings != null)
            {
                foreach (var item in trainings)
                {
                    ITrainingModel trainingModel = factory.CreateTrainingModel(item);
                    trainingModel.IsUserSignedUp = db.IsUserAlreadySignedUpInTraining(email, item.ID);
                    DateTime currentDate = dateTimeService.GetCurrentDate();
                    trainingModel.IsAllowedToSignOff = !(trainingModel.Time - currentDate < TimeSpan.FromHours(4));
                    trainingModel.IsAllowedToSignUp = !(trainingModel.Time <= currentDate);
                
                    model.Trainings.Add(trainingModel);
                }
            }
            return model;
        }
    }
}