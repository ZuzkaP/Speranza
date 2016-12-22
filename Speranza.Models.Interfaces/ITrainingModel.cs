using System;

namespace Speranza.Models.Interfaces
{
    public interface ITrainingModel
    {
        int Capacity { get; set; }
        string Description { get; set; }
        string ID { get; }
        int RegisteredNumber { get; set; }
        DateTime Time { get; set; }
        string Trainer { get; set; }
        bool IsUserSignedUp { get; set; }
        bool IsAllowedToSignedUp { get; set; }
    }
}