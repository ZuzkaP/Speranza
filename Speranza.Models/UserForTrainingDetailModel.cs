﻿using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
   public class UserForTrainingDetailModel : IUserForTrainingDetailModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool HasNoAvailableTrainings { get; set; }

        public bool ParticipationSet
        {
            get; set;
        }
        public DateTime SignUpTime { get ; set ; }
        public bool ParticipationDisapproved { get; set; }

        public int NumberOfFreeSignUps { get; set; }
    }
}
