using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class TrainingsPageModel
    {
       public IList<ITrainingForAdminModel> Trainings { get; set; }
    }
}
