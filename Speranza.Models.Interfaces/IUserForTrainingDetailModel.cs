using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
     public interface IUserForTrainingDetailModel
    {
        string Name { get; set; }
        string Surname { get; set; }
        string Email { get; set; }
        bool HasNoAvailableTrainings { get; set; }
        bool ParticipationSet { get; set; }
        DateTime SignUpTime { get; set; }
        bool ParticipationDisapproved { get; set; }
    }
}
