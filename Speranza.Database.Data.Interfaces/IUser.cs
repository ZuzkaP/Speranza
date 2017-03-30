using Speranza.Common.Data;
using System;

namespace Speranza.Database.Data.Interfaces
{
    public interface IUser
    {
        string PasswordHash { get; }
        string Email { get; }
        string Name { get; }
        string Surname { get; }
        string PhoneNumber { get; }
        bool IsAdmin { get; }
        UserCategories Category { get; set; }
        int NumberOfFreeSignUpsOnSeasonTicket { get; set; }
        int NumberOfPastTrainings { get; set; }
        int NumberOfSignedUpTrainings { get; set; }
        bool ParticipationSet { get; set; }
        DateTime SignUpTime { get; set; }
    }
}