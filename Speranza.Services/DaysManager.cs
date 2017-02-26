using System;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Database;
using System.Linq;

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
            DateTime currentDate = dateTimeService.GetCurrentDate();
            if (trainings != null)
            {
                foreach (var item in trainings)
                {
                    ITrainingModel trainingModel = factory.CreateTrainingModel(item);
                    trainingModel.IsUserSignedUp = db.IsUserAlreadySignedUpInTraining(email, item.ID);
                    trainingModel.IsAllowedToSignOff = !(trainingModel.Time - currentDate < TimeSpan.FromHours(trainingManager.GetSignOffLimit()));
                    trainingModel.IsAllowedToSignUp = !(trainingModel.Time <= currentDate);

                    model.Trainings.Add(trainingModel);
                }
            }

            int day = (int)dateTimeService.GetDayName(date);
            DateTime lastTemplateGeneration = db.GetLastTemplateGenerationDate();
            if (lastTemplateGeneration.Date < date.Date)
            {
                var templates = db.GetTemplatesForTheDay(day);
                if (templates != null && templates.Count > 0)
                {
                    foreach (var item in templates)
                    {
                        if (currentDate.Date == date.Date && currentDate.Hour >= item.Time)
                        {
                            continue;
                        }
                        if (trainings != null && trainings.Any(r => r.Time.Date == date.Date && r.Time.Hour == item.Time))
                        {
                            continue;
                        }
                        if(item.ValidFrom.Date > date.Date)
                        {
                            continue;
                        }
                        var trainingModel = trainingManager.GenerateTrainingFromTemplate(item, date);
                        trainingModel.IsAllowedToSignUp = true;
                        model.Trainings.Add(trainingModel);
                    }
                    db.SetLastTemplateGenerationDate(date);
                }
            }
            return model;
        }
    }
}