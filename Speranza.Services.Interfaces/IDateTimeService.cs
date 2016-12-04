using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services.Interfaces
{
    public interface IDateTimeService
    {
        DateTime GetCurrentDate();
        DayNames GetDayName(DateTime date);
    }
}
