using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
    public interface ITrainingForAdminModel
    {
        int Capacity { get; set; }
        string Description { get; set; }
        string ID { get; set; }
        int RegisteredNumber { get; set; }
        DateTime Time { get; set; }
        string Trainer { get; set; }
    }
}
