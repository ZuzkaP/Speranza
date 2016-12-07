using Speranza.Models;
using Speranza.Database;
using Speranza.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Speranza.Database.Data.Interfaces;
using Speranza.Services.Interfaces;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class AccountsController : Controller
    {
        private IDatabaseGateway db;
        private IHasher hasher;
        private IUserManager manager;
        const int PASSWORD_LENGTH = 6;


        public AccountsController() : this(InMemoryDatabase.Instance,new Hasher(),new UserManager())
        {

        }

        public AccountsController(IDatabaseGateway db, IHasher hasher,IUserManager manager)
        {
            this.db = db;
            this.hasher = hasher;
            this.manager = manager;
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

        public ActionResult SaveUserProfile(UserProfileModel model)
        {
            if (manager.IsUserLoggedIn(Session))
            {
                db.UpdateUserData(model);
                return View(model);
            }
            return RedirectToAction("Index", "Home");
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

        public ActionResult Logout()
        {
            Session["Email"] = null;
            return RedirectToAction("Index", "Home");
         }

        public ActionResult UserProfile()
        {
            if (manager.IsUserLoggedIn(Session))
            {
                IUser user = db.GetUserData((string)Session["Email"]);
                UserProfileModel model = new UserProfileModel();
                model.Email = (string) Session["Email"];
                model.Name = user.Name;
                model.Surname = user.Surname;
                model.PhoneNumber = user.PhoneNumber;
                
                return View(model);

            }
            return RedirectToAction("Index", "Home");
        }
    }
}