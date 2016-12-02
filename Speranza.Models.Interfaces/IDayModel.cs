using System.Collections.Generic;

namespace Speranza.Models.Interfaces
{
    public interface IDayModel
    {
        IList<ITrainingModel> Trainings { get; }
    }
}