using System;

namespace Speranza.Database.Data.Interfaces
{
    public interface ITraining
    {
        int Capacity { get; set; }
        string Description { get; set; }
        int RegisteredNumber { get; set; }
        DateTime Time { get; set; }
        string Trainer { get; set; }
    }
}