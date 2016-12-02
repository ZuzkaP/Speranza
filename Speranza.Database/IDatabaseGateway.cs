using Speranza.Models;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Speranza.Database
{
    public interface IDatabaseGateway
    {
        void RegisterNewUser(RegisterModel model);
        bool UserExists(string email);
        IUser LoadUser(string email);
        IList<ITraining> GetDayTrainings(DateTime date);
    }
}