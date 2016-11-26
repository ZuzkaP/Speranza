using Speranza.Models;
using Speranza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class CalendarController : Controller
    {
        IUserManager userManager;
        IDaysManager dayManager;
        IDateTimeService dateTimeService;


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
            for (int i = 0; i < 14; i++)
            {
                model.Days.Add(dayManager.GetDay(today + TimeSpan.FromDays(i)));
            } 

            return View(model);
        }
    }
}