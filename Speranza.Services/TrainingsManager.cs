using System;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Models;

namespace Speranza.Services
{
    public class TrainingsManager: ITrainingsManager
    {
        public TrainingsManager()
        {
        }

        public ITrainingModel CreateModel(ITraining training)
        {
            ITrainingModel model = new TrainingModel();
            model.Capacity = training.Capacity;
            model.Description = training.Description;
            model.RegisteredNumber = training.RegisteredNumber;
            model.Time = training.Time;
            model.Trainer = training.Trainer;

            return model;
        }
    }
}