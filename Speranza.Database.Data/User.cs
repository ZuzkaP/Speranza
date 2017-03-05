using Speranza.Common.Data;
using Speranza.Database.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data
{
    public class User : IUser
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
        

        public string PhoneNumber { get; set; }

        public bool IsAdmin {  get; set; }

        public UserCategories Category { get; set; }

        public int NumberOfPastTrainings { get; set; }

        public int NumberOfFreeSignUpsOnSeasonTicket { get; set; }

        public int NumberOfSignedUpTrainings { get; set; }

        public bool ParticipationSet
        {
            get; set;
        }
    }
}
