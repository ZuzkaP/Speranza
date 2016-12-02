using System;
using Speranza.Models;
using Speranza.Models.Interfaces;

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