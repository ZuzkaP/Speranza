using Speranza.App_Start;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class PeriodicalAttendancesController : Controller
    {
        private IUserManager userManager;

        public PeriodicalAttendancesController() : this( Initializer.UserManager)
        {

        }

        public PeriodicalAttendancesController(IUserManager userManager)
        {
            this.userManager = userManager;
        }
        // GET: PeriodicalAttendances
        public ActionResult Execute()
        {
            userManager.UpdateSeasonTickets();
            userManager.PromptToConfirmUserAttendance();
            userManager.CleanUpTokens();

            return new EmptyResult();
        }
    }
}