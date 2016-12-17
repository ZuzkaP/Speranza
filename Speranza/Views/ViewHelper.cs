using Speranza.Models;
using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Speranza.Views
{
     public static class ViewHelper
    {
        public static string TranslateDayNames(DayNames dayName)
        {
            switch (dayName)
            {
                case DayNames.Monday: 
                    return "Pondelok";
                case DayNames.Tuesday:
                    return "Utorok";
                case DayNames.Wednesday:
                    return "Streda";
                case DayNames.Thursday:
                    return "Štvrtok";
                case DayNames.Friday:
                    return "Piatok";
                case DayNames.Saturday:
                    return "Sobota";
                case DayNames.Sunday:
                    return "Nedeľa";
                default:
                    throw new InvalidOperationException();
            }

        }

        public static string ShowMessageInView(CalendarMessages message)
        {
            string outputMessage = string.Empty;
            switch (message)
            {
                case CalendarMessages.TrainingDoesNotExist:
                    outputMessage = "Tréning neexistuje!";
                       break;
                case CalendarMessages.TrainingIsFull:
                    outputMessage = "Tréning je už obsadený!";
                       break;
                case CalendarMessages.SignUpSuccessful:
                    outputMessage = "Bol si úspešne prihlásený!";
                       break;
                case CalendarMessages.UserAlreadySignedUp:
                    outputMessage = "Už si prihlásený!";
                       break;
                case CalendarMessages.UserWasSignedOff:
                    outputMessage = "Bol si úspešne odhlásený!";
                       break;
            }

            return outputMessage;
        }
    }
}