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
    }
}