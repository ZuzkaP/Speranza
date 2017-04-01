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
            //var requestContext = HttpContext.Current.Request.RequestContext;
            //urlHelper = new UrlHelper(requestContext);
        }

        public const string WelcomeSubject = "Registrácia";
        public const string WelcomeBody = "Vitajte v štúdiu Speranza. Vaša registrácia bola úspešná.";
        public const string TrainingCanceledBody = "Ahoj, \n ospravedlňujeme sa, ale tréning dňa {0} o {1} je zrušený. Tešíme sa na teba pri ďalšej návšteve. \n Speranza tím";
        public const string TrainingCanceledSubject = "Zrušenie tréningu dňa {0} o {1}";
        public const string AddingUserToTrainingSubject = "Prihlásenie na tréning dňa {0} o {1}";
        public const string AddingUserToTrainingBody = "Ahoj, admin ťa pridal na tréning dňa {0} o {1} \n Speranza";
        public const string RemovingUserFromTrainingBody = "Ahoj, admin ťa odhlásil z tréningu dňa {0} o {1} \n Tešíme sa na tvoju ďalšiu návštevu. \n Speranza";
        public const string RemovingUserFromTrainingSubject = "Odhlásenie z tréningu dňa {0} o {1}";
        public const string ConfirmAttendanceSubject = "Potvrdenie účasti na tréningu dňa {0} o {1}";
        public const string ConfirmAttendanceBody = " Ahoj admin, \npotrvď účasť/neúčasť týchto cvičiacich na tréningu. \n{0}\n\n Tvoja Speranza";
        public static string ConfirmAttendanceSubBody = "{0} {1} <a href=\""+ /*urlHelper.Action("ConfirmParticipation", "AdminPastTrainings") +*/ "?traningId={3}&amp;email={2}\">potvrď účasť</a> / <a href=\""+urlHelper.Action("ConfirmParticipation", "AdminPastTrainings")+"\">potvrď neúčasť</a>";
        private static UrlHelper urlHelper;
        public const string SixthUserSignedUpInTrainingSubject = "Prihlásil sa 6. cvičiaci";
        public const string SixthUserSignedUpInTrainingBody = " Ahoj admin, \nna tréning dňa {0} o {1} sa prihlásil 6. cvičiaci. Skontroluj, či máš trénera.\nTvoja Speranza";
        public const string SixthUserSignedOffFromTrainingSubject = "Odhlásil sa 6. cvičiaci";
        public const string SixthUserSignedOffFromTrainingBody = " Ahoj admin, \nna tréning dňa {0} o {1} sa odhlásil 6. cvičiaci. Nezabudni zrušiť trénera. \nTvoja Speranza";
        public const string RecoveryPassBody = "Ahoj, tvoje nové heslo je <b>{0}</b>\nPo prihlásení si ho nezabudni zmeniť vo svojom profile. \nTvoja Speranza";
        public const string RecoveryPassSubject="Speranza zmena hesla";
    }
}
