using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class AccountsController : Controller
    {
        // GET: Accounts
        public ViewResult Register()
        {
            return View("Register");
        }
    }
}