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
        private ITrainingsManager trainingManager;

        public DaysManager(IDatabaseGateway db, ITrainingsManager trainingManager, IDateTimeService dateTimeService, IModelFactory factory)
        {
            this.db = db;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
            this.factory = factory;
        }

        public IDayModel GetDay(DateTime date, string email)
        {
            IDayModel model = new DayModel(date.ToString("d.M."), dateTimeService.GetDayName(date));
            var trainings = db.GetDayTrainings(date);

            if (trainings != null && trainings.Count>0)
            {
                foreach (var item in trainings)
                {
                    ITrainingModel trainingModel = factory.CreateTrainingModel(item);
                    trainingModel.IsUserSignedUp = db.IsUserAlreadySignedUpInTraining(email, item.ID);
                    DateTime currentDate = dateTimeService.GetCurrentDate();
                    trainingModel.IsAllowedToSignOff = !(trainingModel.Time - currentDate < TimeSpan.FromHours(trainingManager.GetSignOffLimit()));
                    trainingModel.IsAllowedToSignUp = !(trainingModel.Time <= currentDate);
                
                    model.Trainings.Add(trainingModel);
                }
            }
            else
            {
                int day = (int) dateTimeService.GetDayName(date);
                var templates = db.GetTemplatesForTheDay(day);
                var currentDate = dateTimeService.GetCurrentDate();
                foreach (var item in templates)
                {
                    if(currentDate.Date == date.Date && currentDate.Hour >= item.Time)
                    {
                        continue;
                    }
                    var trainingModel = trainingManager.GenerateTrainingFromTemplate(item,date);
                    trainingModel.IsAllowedToSignUp = true;
                    model.Trainings.Add(trainingModel);
                }
            }
            return model;
        }
    }
}