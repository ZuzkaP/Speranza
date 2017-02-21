using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data.Interfaces
{
    public class RecurringTrainingTemplate : IRecurringTrainingTemplate
    {
        public int Capacity { get; set; }
        public int Day { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public string Trainer { get; set; }
        public DateTime ValidFrom { get; set; }
    }
}
