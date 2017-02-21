using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
    public interface IUserProfileModel
    {
        string Name { get; set; }
        string Surname { get; set; }
        string Email { get; set; }
        string PhoneNumber { get; set; }
        ITrainingModel SignedUpOrSignedOffTraining { get; set; }
        CalendarMessages CalendarMessage { get; set; }
        UserProfileMessages UserProfileMessage { get; set; }
        string Category { get; set; }
        int NumberOfFreeSignUps { get; set; }
        int NumberOfPastTrainings { get; set; }
        IList<ITrainingModel> FutureTrainings { get; set; }
        IList<ITrainingModel> PastTrainings { get; set; }
    }
}
