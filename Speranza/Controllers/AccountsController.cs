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
        private ICookieService cookieService;
        private IUidService uidService;

        public AccountsController() : this(Initializer.Db, Initializer.Hasher, Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService, Initializer.Factory, Initializer.CookieService, Initializer.UidService)
        {

        }

        public AccountsController(IDatabaseGateway db, IHasher hasher, IUserManager userManager, ITrainingsManager trainingManager, IDateTimeService dateTimeService, IModelFactory factory, ICookieService cookieService, IUidService uidService)
        {
            this.db = db;
            this.hasher = hasher;
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
            this.factory = factory;
            this.cookieService = cookieService;
            this.uidService = uidService;
        }

        // GET: Accounts
        public ViewResult Register()
        {
            return View("Register");
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            Session["Email"] = null;
            model.LoginSuccessful = false;
            if (model.Password != null)
            {
                string hashPass = hasher.HashPassword(model.Password);

                ILoginResult result = userManager.Login(model.Email, hashPass);

                if (result != null)
                {
                    Session["Email"] = result.Email;
                    Session["IsAdmin"] = result.IsAdmin;
                    Session["Category"] = result.Category;
                    model.LoginSuccessful = true;
                    if(model.RememberMe == true)
                    {
                        string series = uidService.GenerateSeries();
                        string token = uidService.GenerateToken();
                        cookieService.SetRememberMeCookie(Response.Cookies, series, token);
                        userManager.SetRememberMe(result.Email, series, token);
                    }
                    return RedirectToAction("Calendar", "Calendar");
                }
            }
            return View("../Home/Index", model);
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
                userManager.RegisterNewUser(model);
                return View("../Home/Index");
            }

            return View("Register", model);
        }

        public ActionResult SaveUserProfile(UserProfileModel model)
        {
            if (userManager.IsUserLoggedIn(Session))
            {
                if(string.IsNullOrEmpty(model.Surname))
                {
                    Session["Message"] = UserProfileMessages.SurnameIsEmpty;
                    return RedirectToAction("UserProfile");
                }
                if (string.IsNullOrEmpty(model.Name))
                {
                    Session["Message"] = UserProfileMessages.NameIsEmpty;
                    return RedirectToAction("UserProfile");
                }
                model.Email = (string)Session["Email"];
                Session["Message"] = UserProfileMessages.ProfileWasUpdated;
                userManager.UpdateUserData(model);
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
                IUserProfileModel model = userManager.GetUserProfileModelWithDataFromDB((string)Session["Email"]);
                model.FutureTrainings = new List<ITrainingModel>();
                model.PastTrainings = new List<ITrainingModel>();
                model.SignedUpOrSignedOffTraining = (ITrainingModel)Session["Training"];
                OrderAndAssignTrainings(model);

                if (Session["Message"] != null)
                {
                    if (Session["Message"] is UserProfileMessages)
                    {
                        model.UserProfileMessage = (UserProfileMessages)Session["Message"];
                    }
                    else
                    {
                        model.CalendarMessage = (CalendarMessages)Session["Message"];
                    }
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
                string hashOldPass = hasher.HashPassword(oldPass);
                string hashNewPass = hasher.HashPassword(newPass);
                if (user.PasswordHash == hashOldPass)
                {
                    if (string.IsNullOrEmpty(newPass))
                    {
                        return Json(UserProfileMessages.NewPassIsEmpty);
                    }
                    if (string.IsNullOrEmpty(confirmPass))
                    {
                        return Json(UserProfileMessages.ConfirmPassIsEmpty);
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
                    model.OldPass = hashOldPass;
                    model.NewPass = hashNewPass;
                    model.ConfirmPass = null;
                    model.Message = UserProfileMessages.PassWasSucessfullyChanged;
                    userManager.ChangePassword((string)Session["Email"], hashNewPass);
                    return Json(model);
                }
                else
                {
                    return Json(UserProfileMessages.PassAndHashAreNotTheSame);
                }
            }
            return RedirectToAction("Index", "Home");
        }

        private void OrderAndAssignTrainings(IUserProfileModel model)
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
                    trainingModel.IsAllowedToSignOff = !(trainingModel.Time - currentDate < TimeSpan.FromHours(trainingManager.GetSignOffLimit()));

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

        public ViewResult ForgottenPass()
        {
            var message = Session["Message"];
            Session["Message"] = null;
            return View(message);

        }

        public ActionResult SendNewPass(string email)
        {
            bool result = userManager.SendNewPass(email);
            if(!result)
            {
            Session["Message"] = RegisterModelMessages.PasswordRecoveryFailed;
            return RedirectToAction("ForgottenPass", "Accounts");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}