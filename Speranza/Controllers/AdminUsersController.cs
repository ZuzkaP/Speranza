using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class AdminUsersController : Controller
    {
        IUserManager userManager;

        public AdminUsersController(IUserManager userManager)
        {
            this.userManager = userManager;
        }
        // GET: AdminUsers
        public ActionResult AdminUsers()
        {
            if(userManager.IsUserLoggedIn(Session))
            {
                if(Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
                {
                    return RedirectToAction("Calendar", "Calendar");
                }
                return RedirectToAction("AdminUsers", "AdminUsers");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}