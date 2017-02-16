using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
   public class RecurringModel
    {
        public string Trainer { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public List<bool> IsTrainingInTime { get; set; }

        public RecurringModel()
        {
            IsTrainingInTime = new List<bool>();
            for (int i = 0; i < 7*13; i++)
            {
                IsTrainingInTime.Add(false);
            }
            Capacity = 10;
        }

       
    }
}
