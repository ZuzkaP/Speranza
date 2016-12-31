using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class TrainingForAdminModel : ITrainingForAdminModel
    {
       public int Capacity { get; set; }
       public string Description { get; set; }
       public string ID { get; set; }
       public int RegisteredNumber { get; set; }
       public DateTime Time { get; set; }
       public string Trainer { get; set; }
    }
}
