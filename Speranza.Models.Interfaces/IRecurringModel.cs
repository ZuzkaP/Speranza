using System.Collections.Generic;

namespace Speranza.Models.Interfaces
{
    public interface IRecurringModel
    {
        int Capacity { get; set; }
        string Description { get; set; }
        IList<bool> IsTrainingInTime { get; set; }
        string Trainer { get; set; }
    }
}