using System;

namespace Speranza.Database.Data.Interfaces
{
    public interface IUserInTraining
    {
        string Email { get; set; }
        bool ParticipationConfirmed { get; set; }
        bool ParticipationDisproved { get; set; }
        DateTime Time { get; set; }
        string TrainingID { get; set; }
        bool AlreadyProcessed { get; set; }
        bool ZeroEntranceFlag { get; set; }
    }
}