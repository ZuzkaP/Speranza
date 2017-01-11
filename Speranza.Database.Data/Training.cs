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
        public string ID { get; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public int RegisteredNumber { get; set; }
        public DateTime Time { get; set; }
        public string Trainer { get; set; }


        public Training(string id,DateTime dateTime, string description, string trainer, int capacity)
        {
            this.ID = id;
            this.Time = dateTime;
            this.Description = description;
            this.Trainer = trainer;
            this.Capacity = capacity;
        }

      
    }
}
