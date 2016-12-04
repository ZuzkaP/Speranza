using Speranza.Database;
using Speranza.Database.Data.Interfaces;
using Speranza.Models;
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
        IDateTimeService dateTimeService;

        public CalendarController() : this(new UserManager(),new DaysManager(InMemoryDatabase.Instance,new TrainingsManager()),new DateTimeService())
        {

        }

        public CalendarController(IUserManager userManager, IDaysManager dayManager, IDateTimeService dateTimeService)
        {
            this.userManager = userManager;
            this.dayManager = dayManager;
            this.dateTimeService = dateTimeService;
        }
        // GET: Calendar
        public ActionResult Calendar()
        {
           if(!userManager.IsUserLoggedIn(Session))
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
                model.Days.Add(dayManager.GetDay(today + TimeSpan.FromDays(i)));
            } 

            return View(model);
        }
    }
}