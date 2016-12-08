using Speranza.Models.Interfaces;
using System.Collections.Generic;

namespace Speranza.Models
{
    public enum CalendarMessages
    {
        NoMessage,
        TrainingDoesNotExist,
        TrainingIsFull,
        SignUpSuccessful
    }

    public class CalendarModel
    {
        public IList<IDayModel> Days { get; private set; }
        public CalendarMessages Message { get; set; }

        public CalendarModel()
        {
            Days = new List<IDayModel>();
        }
    }
}