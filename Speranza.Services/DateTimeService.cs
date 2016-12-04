using System;
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
    }
}