using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
  
    public class UserProfileModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IList<ITrainingModel> Trainings { get; set; }
        public ITrainingModel SignedUpOrSignedOffTraining { get; set; }
        public CalendarMessages Message { get; set; }
        public string Category { get; set; }
        public int NumberOfFreeSignUps { get; set; }
        public int NumberOfPastTrainings { get; set; }
    }
}
