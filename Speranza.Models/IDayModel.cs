using System.Collections.Generic;

namespace Speranza.Models
{
    public interface IDayModel
    {
        IList<ITrainingModel> Trainings { get; }
    }
}