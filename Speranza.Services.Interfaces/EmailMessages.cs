using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Services.Interfaces
{
   public static class EmailMessages
    {
        static EmailMessages()
        {
        }

        public const string WelcomeSubject = "Registrácia";
        public const string WelcomeBody = "Vitajte v štúdiu Speranza. Vaša registrácia bola úspešná.";
        public const string TrainingCanceledBody = "Ahoj, \nospravedlňujeme sa, ale tréning dňa {0} o {1} je zrušený. Tešíme sa na teba pri ďalšej návšteve. \nSperanza tím";
        public const string TrainingCanceledSubject = "Zrušenie tréningu dňa {0} o {1}";
        public const string AddingUserToTrainingSubject = "Prihlásenie na tréning dňa {0} o {1}";
        public const string AddingUserToTrainingBody = "Ahoj, admin ťa pridal na tréning dňa {0} o {1}. \nTešíme sa na teba. \nTvoja Speranza";
        public const string RemovingUserFromTrainingBody = "Ahoj, admin ťa odhlásil z tréningu dňa {0} o {1} \nTešíme sa na tvoju ďalšiu návštevu. \nSperanza";
        public const string RemovingUserFromTrainingSubject = "Odhlásenie z tréningu dňa {0} o {1}";
        public const string ConfirmAttendanceSubject = "Potvrdenie účasti na tréningoch dňa {0}";
        public const string ConfirmAttendanceBody = " Ahoj admin, \npotrvď účasť/neúčasť cvičiacich na tréningoch: \n{0}\nhttp://treningy.speranza.sk/AdminPastTrainings/AdminTrainings\n\nTvoja Speranza";
        //public static string ConfirmAttendanceSubBody = "{0} {1} <a href=\""+ "http://treningy.speranza.sk/AdminPastTrainings/ConfirmParticipation" + "?traningId={3}&amp;email={2}\">potvrď účasť</a> / <a href=\"http://treningy.speranza.sk/AdminPastTrainings/DisproveParticipation" + "?traningId={3}&amp;email={2}\">potvrď neúčasť</a>";

        public const string SixthUserSignedUpInTrainingSubject = "Prihlásil sa 6. cvičiaci";
        public const string SixthUserSignedUpInTrainingBody = " Ahoj admin, \nna tréning dňa {0} o {1} sa prihlásil 6. cvičiaci. Skontroluj, či máš trénera.\nTvoja Speranza";
        public const string SixthUserSignedOffFromTrainingSubject = "Odhlásil sa 6. cvičiaci";
        public const string SixthUserSignedOffFromTrainingBody = " Ahoj admin, \nna tréning dňa {0} o {1} sa odhlásil 6. cvičiaci. Nezabudni zrušiť trénera. \nTvoja Speranza";
        public const string RecoveryPassBody = "Ahoj, tvoje nové heslo je <b>{0}</b>\nPo prihlásení si ho nezabudni zmeniť vo svojom profile. \nTvoja Speranza";
        public const string RecoveryPassSubject="Speranza zmena hesla";
    }
}
