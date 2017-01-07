using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models.Interfaces;

namespace Speranza.Models
{
    public class TrainingsDetailsModel
    {
        public string Email { get; set; }
        public IList<ITrainingModel> Trainings { get; set; }
    }
}
