using System;
using System.Collections.Generic;
using Speranza.Models;
using Speranza.Models.Interfaces;

namespace Speranza.Models
{
    public class DayModel : IDayModel
    {
        public IList<ITrainingModel> Trainings { get; private set; }

        public string Date { get; private set; }
        public DayNames DayName { get;private set; }

        public DayModel(string date,DayNames dayName)
        {
            Trainings = new List<ITrainingModel>();
            Date = date;
            DayName = dayName;

        }
    }
}