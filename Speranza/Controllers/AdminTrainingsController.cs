using Speranza.App_Start;
using Speranza.Database;
using Speranza.Services;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class AdminTrainingsController : Controller
    {
        IUserManager userManager;

        public AdminTrainingsController(): this(Initializer.UserManager)
        {

        }

        public AdminTrainingsController(IUserManager userManager)
        {
            this.userManager = userManager;
        }

        // GET: AdminTrainings
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