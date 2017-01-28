﻿using Speranza.Models.Interfaces;
using System.Collections.Generic;

namespace Speranza.Models
{

    public class CalendarModel
    {
        public IList<IDayModel> Days { get; private set; }
        public CalendarMessages Message { get; set; }
        public ITrainingModel SignedUpOrSignedOffTraining { get; set; }

        public CalendarModel()
        {
            Days = new List<IDayModel>();
        }
    }
}