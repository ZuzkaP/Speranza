using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
   public interface IAdminTrainingsModel
    {
        IList<ITrainingForAdminModel> Trainings { get; set; }
    }
}
