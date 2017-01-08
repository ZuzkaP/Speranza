using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
   public class UserSignOffModel
    {
        public string Email { get; set; }
        public AdminUsersMessages Message { get; set; }
        public string TrainingDate { get; set; }
        public string TrainingTime { get; set; }
    }
}
