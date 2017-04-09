using System;

namespace Speranza.Database.Data.Interfaces
{
    public interface ITraining
    {
        int Capacity { get; set; }
        string Description { get; set; }
        string ID { get; }
        int RegisteredNumber { get; set; }
        DateTime Time { get; set; }
        string Trainer { get; set; }
        bool IsFromTemplate { get; set; }
    }
}