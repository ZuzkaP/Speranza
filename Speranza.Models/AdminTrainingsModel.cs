using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models.Interfaces;

namespace Speranza.Models
{
    public enum AdminTrainingsMessages
    {
        NoMessage = 0,
        TrainerWasSuccessfullyChanged = 1
    }

    public class AdminTrainingsModel
    {
        public IList<ITrainingForAdminModel> Trainings { get; set; }
    }
}
