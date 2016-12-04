using System.Collections.Generic;

namespace Speranza.Models.Interfaces
{
    public enum DayNames
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday,
        Saturday,
        Sunday
    }
    public interface IDayModel
    {
        string Date {get;}
        DayNames DayName { get;}
        IList<ITrainingModel> Trainings { get; }
    }
}