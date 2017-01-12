using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class CreateTrainingModel
    {
        public AdminTrainingsMessages Message { get; set; }
        public string Trainer { get; set; }
        public string TrainingID { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int Capacity { get; set; }
    }
}
