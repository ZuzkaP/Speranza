using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
    public enum CalendarMessages
    {
        NoMessage = 0,
        TrainingDoesNotExist = 1,
        TrainingIsFull = 2,
        SignUpSuccessful = 3,
        UserAlreadySignedUp = 4,
        UserWasSignedOff = 5,
        UserDoesNotExist = 6
    }
}
