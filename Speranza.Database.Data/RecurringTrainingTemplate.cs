using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data
{
    public class RecurringTrainingTemplate
    {
        public int Capacity { get; set; }
        public int Day { get; set; }
        public string Description { get; set; }
        public int Time { get; set; }
        public string Trainer { get; set; }
    }
}
