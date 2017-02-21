using Speranza.Models;
using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public static string ShowMessageInView(UserProfileMessages message)
        {
            string outputMessage = string.Empty;
            switch (message)
            {
                case UserProfileMessages.ProfileWasUpdated:
                    outputMessage = "<div class=\"alert alert-success\" role=\"alert\">Profil bol aktualizovaný.</div>";
                    break;
                case UserProfileMessages.SurnameIsEmpty:
                    outputMessage = "<div class=\"alert alert-warning\" role=\"alert\">Priezvisko nemôže byť prázdne.</div>";
                    break;
                case UserProfileMessages.NameIsEmpty:
                    outputMessage = "<div class=\"alert alert-warning\" role=\"alert\">Meno nemôže byť prázdne.</div>";
                    break;
            }
            return outputMessage;
        }


        public static string ShowMessageInView(RecurringTrainingMessages message)
        {
            string outputMessage = string.Empty;
            switch (message)
            {
                case RecurringTrainingMessages.NoCapacity:
                    outputMessage = "<div class=\"alert alert-danger\" id=\"MessageBoxCreating\" role=\"alert\">Kapacita musí byť zadaná.</div>";
                    break;
                case RecurringTrainingMessages.NoDescription:
                    outputMessage = "<div class=\"alert alert-danger\" id=\"MessageBoxCreating\" role=\"alert\">Popis musí byť zadaný.</div>";
                    break;
                case RecurringTrainingMessages.NoTrainer:
                    outputMessage = "<div class=\"alert alert-danger\" id=\"MessageBoxCreating\" role=\"alert\">Tréner musí byť zadaný.</div>";
                    break;
                case RecurringTrainingMessages.NoModel:
                    outputMessage = "<div class=\"alert alert-danger\" id=\"MessageBoxCreating\" role=\"alert\">Neočakávaná chyba, skúste znova.</div>";
                    break;
                case RecurringTrainingMessages.Success:
                    outputMessage = "<div class=\"alert alert-success\" id=\"MessageBoxCreating\" role=\"alert\">Opakujúce sa tréningy boli úspešne nastavené.</div>";
                    break;
            }
            return outputMessage;
        }

        public static IList<SelectListItem> CreateSelectListItems(IList<string> items,string selectedItem)
        {
            return items.Select(r => new SelectListItem() { Text = r, Selected = r == selectedItem}).ToList();
        }
    }
}