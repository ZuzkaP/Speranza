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

        public static string ShowMessageInView(CalendarMessages message,ITrainingModel training)
        {
            string outputMessage = string.Empty;
            switch (message)
            {
                case CalendarMessages.TrainingDoesNotExist:
                    outputMessage = "<div class=\"alert alert-warning\" role=\"alert\">Tréning neexistuje!</div>";
                       break;
                case CalendarMessages.TrainingIsFull:
                    outputMessage = "<div class=\"alert alert-warning\" role=\"alert\">Tréning je už obsadený!</div>";
                       break;
                case CalendarMessages.SignUpSuccessful:
                    outputMessage = string.Format("<div class=\"alert alert-success\" role=\"alert\">Bol si úspešne prihlásený na tréning dňa {0} o {1}!</div>", training.Time.ToString("dd.MM.yyyy"), training.Time.ToString("HH:mm"));
                    break;
                case CalendarMessages.UserAlreadySignedUp:
                    outputMessage = "<div class=\"alert alert-warning\" role=\"alert\">Už si prihlásený!</div>";
                       break;
                case CalendarMessages.UserWasSignedOff:
                    outputMessage = string.Format("<div class=\"alert alert-info\" role=\"alert\">Bol si úspešne odhlásený z tréningu dňa {0} o {1}!</div>", training.Time.ToString("dd.MM.yyyy"),training.Time.ToString("HH:mm"));
                       break;
            }

            return outputMessage;
        }
    }
}