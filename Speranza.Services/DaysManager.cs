using System;
using Speranza.Models;

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