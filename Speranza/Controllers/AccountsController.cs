using Speranza.Models;
using Speranza.Database;
using Speranza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;

namespace Speranza.Controllers
{
    public class AccountsController : Controller
    {
        private IDatabaseGateway db;
        private IHasher hasher;
        const int PASSWORD_LENGTH = 6;

        public AccountsController() : this(InMemoryDatabase.Instance,new Hasher())
        {

        }

        public AccountsController(IDatabaseGateway db, IHasher hasher)
        {
            this.db = db;
            this.hasher = hasher;
        }

        // GET: Accounts
        public ViewResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            Session["Email"] = string.Empty;
            model.LoginSuccessful = false;

            if (!string.IsNullOrEmpty(model.Email))
            {
                IUser user = db.LoadUser(model.Email);
                if (user != null)
                {
                    string hashPass = hasher.HashPassword(model.Password);
                    if (hashPass == user.PasswordHash)
                    {
                        Session["Email"] = model.Email;
                        model.LoginSuccessful = true;
                        return RedirectToAction("Calendar", "Calendar");
                       // return View("../Calendar/Calendar", model);
                    }
                }

            }
            return View("Index", "Home", model);
        }

        [HttpPost]
        public ViewResult Register(RegisterModel model)
        {

            if (String.IsNullOrEmpty(model.Email))
            {
                model.Messages |= RegisterModelMessages.EmailIsEmpty;
            }

            else if (!CheckEmailFormat(model.Email))
            {
                model.Messages |= RegisterModelMessages.EmailFormatIsIncorrect;
            }

            if (String.IsNullOrEmpty(model.Password))
            {
                model.Messages |= RegisterModelMessages.PasswordIsEmpty;
            }

            else
            {
                if (model.Password.Length < PASSWORD_LENGTH)
                {
                    model.Messages |= RegisterModelMessages.PasswordIsTooShort;
                }

                if (!model.Password.Any(char.IsDigit))
                {
                    model.Messages |= RegisterModelMessages.PasswordHasNoNumber;
                }

                if (!model.Password.Any(char.IsLetter))
                {
                    model.Messages |= RegisterModelMessages.PasswordHasNoLetter;
                }
                if (model.Password != model.ConfirmPassword)
                {
                    model.Messages |= RegisterModelMessages.ConfirmPassIncorrect;
                }
            }

            if (model.Messages == RegisterModelMessages.NoMessage && db.UserExists(model.Email) == true)
            {
                model.Messages |= RegisterModelMessages.UserAlreadyExists;
            }

            if (model.Messages == RegisterModelMessages.NoMessage)
            {
                model.Password = hasher.HashPassword(model.Password);
                model.ConfirmPassword = null;
                db.RegisterNewUser(model);
                return View("../Home/Index");
            }

            return View("Register", model);
        }

        private bool CheckEmailFormat(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,6}$";
            Regex regex = new Regex(emailPattern);
            if (!regex.IsMatch(email))
            {
                return false;
            }
            return true;
        }
    }
}