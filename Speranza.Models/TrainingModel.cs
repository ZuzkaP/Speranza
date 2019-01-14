using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class TrainingModel : ITrainingModel
    {

        public TrainingModel(string id)
        {
            this.ID = id;
        }
        public int Capacity { get; set; }
        public string Description { get; set; }
        public string ID { get; }

        public int RegisteredNumber { get; set; }
        public DateTime Time { get; set; }
        public string Trainer { get; set; }
        public bool IsUserSignedUp { get; set; }
        public bool IsAllowedToSignUp { get; set; }
        public bool IsAllowedToSignOff { get; set; }
        public string DateStr { get; set; }
        public string TimeStr { get; set; }


    }
}
