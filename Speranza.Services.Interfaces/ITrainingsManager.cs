using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface ITrainingsManager
    {
        ITrainingModel CreateModel(ITraining training);
    }
}