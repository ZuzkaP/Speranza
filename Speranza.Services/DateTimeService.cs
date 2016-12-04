using System;
using Speranza.Models.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;

namespace Speranza.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }

        public DayNames GetDayName(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}