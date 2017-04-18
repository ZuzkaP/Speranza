using Speranza.App_Start;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class HomeController : Controller
    {
        private ICookieService cookieService;
        private IUserManager userManager;

        public HomeController(ICookieService cookieService, IUserManager userManager)
        {
            this.cookieService = cookieService;
            this.userManager = userManager;
        }

        public HomeController() 
            :this(Initializer.CookieService,Initializer.UserManager)
        {

        }
        // GET: Home
        public ActionResult Index()
        {
           string cookie = cookieService.GetRememberMeCookie(Request.Cookies);
            if(!string.IsNullOrEmpty(cookie))
            {
               var user = userManager.VerifyRememberMe(cookie);
                if(user != null)
                {
                    Session["Email"] = user.Email;
                    Session["Category"] = user.Category;
                    Session["IsAdmin"] = user.IsAdmin;
                    return RedirectToAction("Calendar", "Calendar");
                }
            }
            return View("Index");
        }

        // GET: Home
        public ViewResult Speranza()
        {
            return View();
        }
    }
}