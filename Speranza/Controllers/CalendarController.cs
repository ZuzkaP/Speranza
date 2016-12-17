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
        private IDatabaseGateway db;

        public CalendarController() : this(InMemoryDatabase.Instance, new UserManager(), new DaysManager(InMemoryDatabase.Instance, new TrainingsManager(), new DateTimeService()), new DateTimeService(), new TrainingsManager())
        {

        }
        public CalendarController(IDatabaseGateway db, IUserManager userManager, IDaysManager dayManager, IDateTimeService dateTimeService, ITrainingsManager trainingManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.dayManager = dayManager;
            this.dateTimeService = dateTimeService;
            this.trainingManager = trainingManager;
        }

        public RedirectToRouteResult SignUp(string id)
        {
            if (!userManager.IsUserLoggedIn(Session))
            {
                return RedirectToAction("Index", "Home");
            }
            ITraining training = db.GetTrainingData(id);

            if (training == null)
            {
                Session["Message"] = CalendarMessages.TrainingDoesNotExist;
                return RedirectToAction("Calendar");
            }
            if (training.RegisteredNumber >= training.Capacity)
            {
                Session["Message"] = CalendarMessages.TrainingIsFull;
                return RedirectToAction("Calendar");
            }
            if (db.IsUserAlreadySignedUpInTraining((string)Session["Email"], id))
            {
                Session["Message"] = CalendarMessages.UserAlreadySignedUp;
                return RedirectToAction("Calendar");
            }
            db.AddUserToTraining((string)Session["Email"], id);
            Session["Message"] = CalendarMessages.SignUpSuccessful;
            return RedirectToAction("Calendar");
        }


        public ActionResult Calendar()
        {
            if (!userManager.IsUserLoggedIn(Session))
            {
                return RedirectToAction("Index", "Home");
            }
            CalendarModel model = new CalendarModel();
            DateTime today = dateTimeService.GetCurrentDate();
            int daysCount =
                userManager.GetUserCategory(Session) == UserCategories.Silver ?
                SILVER_USERS_DAYS :
                userManager.GetUserCategory(Session) == UserCategories.Gold ?
                GOLDEN_USERS_DAYS :
                STANDARD_USERS_DAYS
                ;

            for (int i = 0; i < daysCount; i++)
            {
                model.Days.Add(dayManager.GetDay(today + TimeSpan.FromDays(i), (string)Session["Email"]));
            }
            model.Message = CalendarMessages.NoMessage;
            model.SignedUpOrSignedOffTraining = (ITrainingModel) Session["Training"];
            if (Session["Message"] != null)
            {
                model.Message = (CalendarMessages)Session["Message"];
            }
            Session["Message"] = null;
            return View(model);
        }

        public RedirectToRouteResult SignOff(string id)
        {
            if (!userManager.IsUserLoggedIn(Session))
            {
                return RedirectToAction("Index", "Home");
            }

            db.RemoveUserFromTraining((string)Session["Email"], id);
            Session["Message"] = CalendarMessages.UserWasSignedOff;
            ITraining training = db.GetTrainingData(id);
            Session["Training"] = trainingManager.CreateModel(training);

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