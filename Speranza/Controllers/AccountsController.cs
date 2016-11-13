using Speranza.Models;
using Speranza.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class AccountsController : Controller
    {
        private IDatabaseGateway db;

        public AccountsController(IDatabaseGateway db)
        {
            this.db = db;
        }

        // GET: Accounts
        public ViewResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ViewResult Register(RegisterModel model)
        {
            if(String.IsNullOrEmpty(model.Email))
            {
                model.Messages |= RegisterModelMessages.EmailIsEmpty;
            }
            return View("Register",model);
        }
    }
}