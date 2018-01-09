using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Speranza.App_Start;
using Speranza.Services.Interfaces;

namespace Speranza.Controllers
{
    public class GalleryController : Controller
    {
        private ICookieService cookieService;

        private IUserManager userManager;

        public GalleryController() : this(Initializer.UserManager,Initializer.CookieService)
        {
                
        }

        public GalleryController(IUserManager userManager,ICookieService cookieService)
        {
            this.cookieService = cookieService;
            this.userManager = userManager;
        }

        // GET: Gallery
        public ActionResult Index()
        {
            if (!userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies), Session))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}