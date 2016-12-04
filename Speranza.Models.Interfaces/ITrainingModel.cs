using System;

namespace Speranza.Models.Interfaces
{
    public interface ITrainingModel
    {
        int Capacity { get; set; }
        string Description { get; set; }
        int RegisteredNumber { get; set; }
        DateTime Time { get; set; }
        string Trainer { get; set; }
    }
}