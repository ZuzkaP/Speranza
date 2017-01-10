using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
   public class UsersInTrainingModel
    {
        public IList<IUserForTrainingDetailModel> Users { get; set; }

        public string TrainingID { get; set; }
    }
}
