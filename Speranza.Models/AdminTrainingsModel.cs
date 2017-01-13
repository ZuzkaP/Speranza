﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models.Interfaces;

namespace Speranza.Models
{
    public enum AdminTrainingsMessages
    {
        NoMessage = 0,
        TraininingDescriptionWasSuccessfullyChanged = 1,
        TrainerWasSuccessfullyChanged = 2,
        TraininingCapacityWasSuccessfullyChanged = 3,
        TraininingCapacityCannotBeLessThanOne = 4,
        NewTrainingNoTrainer = 5,
        NewTrainingNoDescription = 6,
        NewTrainingDateInvalid = 7,
        NewTrainingTimeInvalid = 8,
        NewTrainingSuccessfullyCreated = 9,
        TrainingWasCanceled = 10,
        TrainingIDInvalid = 11,
        NewTrainingDateInPast = 12
    }

    public class AdminTrainingsModel : IAdminTrainingsModel
    {
        public IList<ITrainingForAdminModel> Trainings { get; set; }
    }
}
