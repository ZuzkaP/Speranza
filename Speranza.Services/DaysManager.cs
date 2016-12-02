using System;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;

namespace Speranza.Services
{
    public class DaysManager : IDaysManager
    {
        public DaysManager()
        {
        }

        public IDayModel GetDay(DateTime date)
        {
            return new DayModel();
        }
    }
}