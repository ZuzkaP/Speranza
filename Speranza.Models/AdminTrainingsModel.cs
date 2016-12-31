using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models.Interfaces;

namespace Speranza.Models
{
    public class AdminTrainingsModel
    {
        public IList<ITrainingForAdminModel> Trainings { get; set; }
    }
}
