using System;
using System.Collections.Generic;
using Speranza.Models;

namespace Speranza.Models
{
    public class DayModel : IDayModel
    {
        public IList<ITrainingModel> Trainings { get; private set; }

        public DayModel()
        {
            Trainings = new List<ITrainingModel>();

        }
    }
}