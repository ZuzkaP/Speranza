using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services.Interfaces
{
   public static class EmailMessages
    {
        public const string WelcomeSubject = "Registrácia";
        public const string WelcomeBody = "Vitajte v štúdiu Speranza. Vaša registrácia bola úspešná.";
        public const string TrainingCanceledBody = "Ahoj, \n ospravedlňujeme sa, ale tréning dňa {0} o {1} je zrušený. Tešíme sa na teba pri ďalšej návšteve. \n Speranza tím";
        public const string TrainingCanceledSubject = "Zrušenie tréningu dňa {0} o {1}";
        public const string AddingUserToTrainingSubject = "Prihlásenie na tréning dňa {0} o {1}";
        public const string AddingUserToTrainingBody = "Ahoj, admin ťa pridal na tréning dňa {0} o {1} \n Speranza";
    }
}
