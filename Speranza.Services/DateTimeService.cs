using System;
using Speranza.Models.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;
using Speranza.Services.Interfaces.Exceptions;

namespace Speranza.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }
        public DateTime GetCurrentDate()
        {
            return DateTime.Now.Date;
        }

        public DayNames GetDayName(DateTime date)
        {
            return (DayNames) Enum.Parse(typeof(DayNames),date.DayOfWeek.ToString());
        }

        public DateTime ParseDateTime(string date, string time)
        {
            if(string.IsNullOrEmpty(date))
            {
                throw new InvalidDateException();
            }
            if(string.IsNullOrEmpty(time))
            {
                throw new InvalidTimeException();
            }
            DateTime resultDate;
            if (!DateTime.TryParse(date, out resultDate))
            {
                throw new InvalidDateException();
            }
            DateTime resultTime;
            if (!DateTime.TryParse(time, out resultTime))
            {
                throw new InvalidTimeException();
            }
            return resultDate.AddHours(resultTime.Hour).AddMinutes(resultTime.Minute);
        }

        public DateTime ParseDate(string date)
        {
            DateTime parsedDate;
            if (!DateTime.TryParse(date, out parsedDate))
            {
                throw new InvalidTimeException();
            }

            return parsedDate;
        }
    }
}