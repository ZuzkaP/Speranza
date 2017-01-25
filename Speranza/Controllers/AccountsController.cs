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
using Speranza.App_Start;

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
        private IModelFactory factory;

        public AccountsController() : this(Initializer.Db, Initializer.Hasher, Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService,Initializer.Factory )
        {

        }

        public AccountsController(IDatabaseGateway db, IHasher hasher,IUserManager userManager,ITrainingsManager trainingManager, IDateTimeService dateTimeService, IModelFactory factory)
        {
            this.db = db;
            this.hasher = hasher;
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
            this.factory = factory;
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
                        Session["Category"] = user.Category;
                        model.LoginSuccessful = true;
                        return RedirectToAction("Calendar", "Calendar");
                    }
                }
            }
            return View("../Home/Index",model);
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
                model.Category = user.Category.ToString();
                model.NumberOfFreeSignUps = user.NumberOfFreeSignUpsOnSeasonTicket;
                model.NumberOfPastTrainings = user.NumberOfPastTrainings;
                model.FutureTrainings = new List<ITrainingModel>();
                model.PastTrainings = new List<ITrainingModel>();
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

 
        public ActionResult ChangeUserPassword(string oldPass, string newPass, string confirmPass)
        {
            if (userManager.IsUserLoggedIn(Session))
            {
                IUser user = db.LoadUser((string)Session["Email"]);

                if (user.PasswordHash == hasher.HashPassword(oldPass))
                {
                    if(string.IsNullOrEmpty(newPass))
                    {
                        return Json(UserProfileMessages.NewPassIsEmpty);
                    }
                    if (string.IsNullOrEmpty(confirmPass))
                    {
                        return Json(UserProfileMessages.ConfirmPassIsEMpty);
                    }
                    if (newPass.Length < PASSWORD_LENGTH)
                    {
                        return Json(UserProfileMessages.NewPassIsTooShort);
                    }
                    if (!newPass.Any(char.IsDigit))
                    {
                        return Json(UserProfileMessages.NewPassHasNoNumber);
                    }

                    if (!newPass.Any(char.IsLetter))
                    {
                        return Json(UserProfileMessages.NewPassHasNoLetter);

                    }
                    if (newPass != confirmPass)
                    {
                        return Json(UserProfileMessages.NewPassAndConfirmPassAreNotTheSame);

                    }
                    ChangePassModel model = new ChangePassModel();
                    model.Email = (string)Session["Email"];
                    model.OldPass = oldPass;
                    model.NewPass = newPass;
                    model.ConfirmPass = confirmPass;
                    model.Message = UserProfileMessages.PassWasSucessfullyChanged;

                    return Json(model);
                }
                else
                {
                    return Json(UserProfileMessages.PassAndHashAreNotTheSame);
                }
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
                    ITrainingModel trainingModel = factory.CreateTrainingModel(item);
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
                model.FutureTrainings = futureTrainings.OrderBy(r => r.Time).ToList();
                model.PastTrainings = pastTrainings.OrderByDescending(r => r.Time).ToList();
            }
        }
    }
}