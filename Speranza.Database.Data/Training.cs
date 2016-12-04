using Speranza.Database.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data
{
   public class Training : ITraining
    {
        public int Capacity { get; set; }
        public string Description { get; set; }
        public int RegisteredNumber { get; set; }
        public DateTime Time { get; set; }
        public string Trainer { get; set; }

        public Training(DateTime dateTime, string v1, string v2, int v3, int v4)
        {
            this.Time = dateTime;
            this.Description = v1;
            this.Trainer = v2;
            this.Capacity = v3;
            this.RegisteredNumber = v4;
        }

      
    }
}
