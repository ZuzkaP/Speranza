using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Speranza.Services.Interfaces;
using Speranza.App_Start;

namespace Speranza.Controllers
{
    public class AdminPastTrainingsController : Controller
    {
        private IUserManager userManager;
        private ITrainingsManager trainingManager;
        private IDateTimeService dateTimeService;

        public AdminPastTrainingsController(IUserManager userManager, ITrainingsManager trainingManager, IDateTimeService dateTimeService)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
        }

        public AdminPastTrainingsController() : this(Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService)
        {

        }

        // GET: AdminPastTrainings
        public ActionResult AdminTrainings()
        {
            if (userManager.IsUserLoggedIn(Session))
            {
                if (!userManager.IsUserAdmin(Session))
                {
                    return RedirectToAction("Calendar", "Calendar");
                }
                return View("AdminTrainings");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}