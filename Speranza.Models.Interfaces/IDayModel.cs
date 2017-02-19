using System.Collections.Generic;

namespace Speranza.Models.Interfaces
{
    public enum DayNames
    {
        Monday = 0,
        Tuesday = 1,
        Wednesday = 2,
        Thursday = 3,
        Friday = 4,
        Saturday = 5,
        Sunday = 6
    }
    public interface IDayModel
    {
        string Date {get;}
        DayNames DayName { get;}
        IList<ITrainingModel> Trainings { get; }
    }
}