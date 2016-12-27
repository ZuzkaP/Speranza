using Speranza.Models;
using Speranza.Database;
using Speranza.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Speranza.Database.Data.Interfaces;
using Speranza.Services.Interfaces;
using System.Web.Mvc;
using Speranza.Models.Interfaces;
using System.Collections.Generic;

namespace Speranza.Controllers
{
    public class AccountsController : Controller
    {
        private IDatabaseGateway db;
        private IHasher hasher;
        private IUserManager userManager;
        private ITrainingsManager trainingManager;
        private IDateTimeService dateTimeService;
        const int PASSWORD_LENGTH = 6;

        public AccountsController() : this(InMemoryDatabase.Instance,new Hasher(),new UserManager(),new TrainingsManager(),new DateTimeService())
        {

        }

        public AccountsController(IDatabaseGateway db, IHasher hasher,IUserManager userManager,ITrainingsManager trainingManager, IDateTimeService dateTimeService)
        {
            this.db = db;
            this.hasher = hasher;
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
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
                        Session["IsAdmin"] = user.IsAdmin;
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
            if (userManager.IsUserLoggedIn(Session))
            {
                model.Email = (string) Session["Email"];
                db.UpdateUserData(model);
                return RedirectToAction("UserProfile");
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
            if (userManager.IsUserLoggedIn(Session))
            {
                IUser user = db.GetUserData((string)Session["Email"]);
                UserProfileModel model = new UserProfileModel();
                model.Email = (string) Session["Email"];
                model.Name = user.Name;
                model.Surname = user.Surname;
                model.PhoneNumber = user.PhoneNumber;
                model.Trainings = new List<ITrainingModel>();
                model.SignedUpOrSignedOffTraining = (ITrainingModel) Session["Training"];
                OrderAndAssignTrainings(model);
               
                if (Session["Message"] != null)
                {
                    model.Message = (CalendarMessages)Session["Message"];
                }
                Session["Message"] = null;
                Session["Training"] = null;
                return View(model);

            }
            return RedirectToAction("Index", "Home");
        }

        private void OrderAndAssignTrainings(UserProfileModel model)
        {
            IList<ITraining> trainings = db.GetTrainingsForUser((string)Session["Email"]);
            if (trainings != null)
            {
                List<ITrainingModel> futureTrainings = new List<ITrainingModel>();
                List<ITrainingModel> pastTrainings = new List<ITrainingModel>();
                foreach (var item in trainings)
                {
                    ITrainingModel trainingModel = trainingManager.CreateModel(item);
                    DateTime currentDate = dateTimeService.GetCurrentDate();
                    trainingModel.IsAllowedToSignOff = !(trainingModel.Time - currentDate < TimeSpan.FromHours(4));
                    
                    if (trainingModel.Time < dateTimeService.GetCurrentDate())
                    {
                        pastTrainings.Add(trainingModel);
                    }
                    else
                    {
                        futureTrainings.Add(trainingModel);
                    }
                }
                model.Trainings = futureTrainings.OrderBy(r => r.Time).Concat(pastTrainings.OrderByDescending(r => r.Time)).ToList();
            }
        }
    }
}