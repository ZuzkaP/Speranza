using Speranza.Models.Interfaces;
using System.Collections.Generic;

namespace Speranza.Models
{
    public enum CalendarMessages
    {
        NoMessage = 0,
        TrainingDoesNotExist = 1,
        TrainingIsFull = 2,
        SignUpSuccessful = 3,
        UserAlreadySignedUp = 4,
        UserWasSignedOff = 5
    }

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