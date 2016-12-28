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

        public AdminTrainingsController(): this(new UserManager())
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
                if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                {
                    return RedirectToAction("Calendar", "Calendar");
                }
                return View();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}