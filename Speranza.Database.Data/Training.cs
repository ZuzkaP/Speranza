﻿using Speranza.Database.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data
{
   public class Training : ITraining
    {
        public string ID { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public int RegisteredNumber { get; set; }
        public DateTime Time { get; set; }
        public string Trainer { get; set; }
        public bool IsFromTemplate { get; set; }

        public Training()
        {
                
        }

        public Training(string id,DateTime dateTime, string description, string trainer, int capacity, bool isFromTemplate)
        {
            this.ID = id;
            this.Time = dateTime;
            this.Description = description;
            this.Trainer = trainer;
            this.Capacity = capacity;
            this.IsFromTemplate = isFromTemplate;
        }

      
    }
}
