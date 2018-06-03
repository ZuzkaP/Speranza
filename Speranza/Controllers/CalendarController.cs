using Speranza.App_Start;
using Speranza.Common.Data;
using Speranza.Database;
using Speranza.Database.Data.Interfaces;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class CalendarController : Controller
    {
        private const int SILVER_USERS_DAYS = 30;
        private const int GOLDEN_USERS_DAYS = 60;
        private const int STANDARD_USERS_DAYS = 14;
        IUserManager userManager;
        IDaysManager dayManager;
        ITrainingsManager trainingManager;
        IDateTimeService dateTimeService;
        IMessageManager messageManager;
        private IDatabaseGateway db;
        private IModelFactory factory;
        private ICookieService cookieService;

        public CalendarController() : this(Initializer.Db,Initializer.UserManager, Initializer.DaysManager, Initializer.DateTimeService, Initializer.TrainingsManager, Initializer.Factory, Initializer.CookieService, Initializer.MessageManager)
        {

        }
        public CalendarController(IDatabaseGateway db, IUserManager userManager, IDaysManager dayManager, IDateTimeService dateTimeService, ITrainingsManager trainingManager, IModelFactory factory, ICookieService cookieService, IMessageManager messageManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.dayManager = dayManager;
            this.dateTimeService = dateTimeService;
            this.trainingManager = trainingManager;
            this.factory = factory;
            this.cookieService = cookieService;
            this.messageManager = messageManager;
        }

        public RedirectToRouteResult SignUp(string id)
        {
            if (!userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies), Session))
            {
                return RedirectToAction("Index", "Home");
            }

           CalendarMessages message = trainingManager.AddUserToTraining((string)Session["Email"], id,dateTimeService.GetCurrentDateTime());
            if(message ==  CalendarMessages.SignUpSuccessful)
            {
                ITraining training = db.GetTrainingData(id);
                Session["Message"] = CalendarMessages.SignUpSuccessful;
                Session["Training"] = factory.CreateTrainingModel(training);
                return RedirectToAction("Calendar");
            }
            Session["Message"] = message;
            return RedirectToAction("Calendar");
        }


        public ActionResult Calendar()
        {
            if (!userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies), Session))
            {
                return RedirectToAction("Index", "Home");
            }
            var userEmail = (string)Session["Email"];
            Session["Category"] = userManager.UpdateUserCategory(userEmail,(UserCategories)Session["Category"]);
            CalendarModel model = new CalendarModel();
            DateTime today = dateTimeService.GetCurrentDateTime();
            int daysCount =
                userManager.GetUserCategory(Session) == UserCategories.Silver ?
                SILVER_USERS_DAYS :
                userManager.GetUserCategory(Session) == UserCategories.Gold ?
                GOLDEN_USERS_DAYS :
                STANDARD_USERS_DAYS
                ;

            
            for (int i = 0; i < daysCount; i++)
            {
                model.Days.Add(dayManager.GetDay(today + TimeSpan.FromDays(i), userEmail));
            }
            model.AllowToSignUp = userManager.GetAllowedToSignUpFlag(userEmail);
            model.Message = CalendarMessages.NoMessage;
            model.SignedUpOrSignedOffTraining = (ITrainingModel) Session["Training"];
            model.UserInfoMessage.Message = messageManager.GetMessageForCurrentDate().Message ?? "";
            if (Session["Message"] != null)
            {
                model.Message = (CalendarMessages)Session["Message"];
            }
            Session["Message"] = null;
            Session["Training"] = null; 
            return View(model);
        }

        public RedirectToRouteResult SignOff(string id)
        {
            if (!userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies), Session))
            {
                return RedirectToAction("Index", "Home");
            }

            ITrainingModel model = trainingManager.RemoveUserFromTraining((string)Session["Email"], id);
            Session["Message"] = CalendarMessages.UserWasSignedOff;
            Session["Training"] = model;

            if (Request.UrlReferrer != null)
            {
                string path = Request.UrlReferrer.AbsolutePath;
                if (path.Contains("Accounts/UserProfile"))
                {
                    return RedirectToAction("UserProfile", "Accounts");
                }
            }
            
            return RedirectToAction("Calendar");

        }
    }
}