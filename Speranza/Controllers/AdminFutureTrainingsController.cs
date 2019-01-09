using Speranza.App_Start;
using Speranza.Database;
using Speranza.Models;
using Speranza.Models.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;
using Speranza.Services.Interfaces.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Speranza.Controllers
{
    public class AdminFutureTrainingsController : Controller
    {
        private const int DEFAULT_PAGE_SIZE = 20;
        private ITrainingsManager trainingManager;
        private IUserManager userManager;
        private IDateTimeService dateTimeService;
        private IUserDataParser parser;
        private ICookieService cookieService;

        public AdminFutureTrainingsController() : this(Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService, Initializer.UserDataParser, Initializer.CookieService)
        {

        }

        public AdminFutureTrainingsController(IUserManager userManager, ITrainingsManager trainingManager, IDateTimeService dateTimeService, IUserDataParser parser, ICookieService cookieService)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
            this.parser = parser;
            this.cookieService = cookieService;
        }

        // GET: AdminTrainings
        public ActionResult AdminTrainings(int? pageSize =  null)
        {
            if (userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies), Session))
            {
                if (!userManager.IsUserAdmin(Session))
                {
                    return RedirectToAction("Calendar", "Calendar");
                }

                var setPageSize = pageSize.HasValue ? pageSize.Value : DEFAULT_PAGE_SIZE;
                int numberOfPages = (int)Math.Ceiling(trainingManager.GetFutureTrainingsCount() / (double)setPageSize);
                IList<ITrainingForAdminModel> trainings = trainingManager.GetFutureTrainings(0, setPageSize);
                IList<IUserForTrainingDetailModel> users = userManager.GetAllUsersForTrainingDetails();
                int signOffLimit = trainingManager.GetSignOffLimit();
                AdminTrainingsModel model = new AdminTrainingsModel();
                model.Trainings = trainings;
                model.Users = users;
                model.SignOffLimit = signOffLimit;
                model.PagesCount = numberOfPages;
                model.PageNumber = 1;
                model.PageSize = setPageSize;
                return View("AdminTrainings", model);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeTrainer(string trainingID, string trainer)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(trainer) || string.IsNullOrEmpty(trainingID))
            {
                return Json("");
            }
            trainingManager.SetTrainer(trainingID, trainer);

            return Json(AdminTrainingsMessages.TrainerWasSuccessfullyChanged);
        }

        public ActionResult ChangeTrainingDescription(string trainingID, string description)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(trainingID))
            {
                return Json("");
            }
            trainingManager.SetTrainingDescription(trainingID, description);

            return Json(AdminTrainingsMessages.TraininingDescriptionWasSuccessfullyChanged);
        }

        public ActionResult ChangeTrainingCapacity(string trainingID, int capacity)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(trainingID))
            {
                return Json("");
            }
            if (capacity < 1)
            {
                return Json(AdminTrainingsMessages.TraininingCapacityCannotBeLessThanOne);
            }
            if(trainingManager.GetAllUsersInTraining(trainingID).Count > capacity)
            {
                return Json(AdminTrainingsMessages.TraininingCapacityLowerThanCountOfSignedUpUsers);
            }

            trainingManager.SetTrainingCapacity(trainingID, capacity);

            return Json(AdminTrainingsMessages.TraininingCapacityWasSuccessfullyChanged);
        }


        public ActionResult TrainingDetails(string trainingID)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(trainingID))
            {
                return Json("");
            }
            IList<IUserForTrainingDetailModel> users = trainingManager.GetAllUsersInTraining(trainingID);
            UsersInTrainingModel model = new UsersInTrainingModel();
            model.TrainingID = trainingID;
            model.Users = users.OrderBy(r=>r.SignUpTime).ToList();

            return PartialView("UsersInTraining", model);
        }

        public ActionResult CreateNewTraining(string date, string time, string trainer, string description,int capacity)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(trainer))
            {
                return Json(AdminTrainingsMessages.NewTrainingNoTrainer);
            }
            if (string.IsNullOrEmpty(description))
            {
                return Json(AdminTrainingsMessages.NewTrainingNoDescription);
            }
            if (capacity < 1)
            {
                return Json(AdminTrainingsMessages.TraininingCapacityCannotBeLessThanOne);
            }
            try
            {
                DateTime dateTime = dateTimeService.ParseDateTime(date, time);
                if(dateTime < dateTimeService.GetCurrentDateTime())
                {
                    return Json(AdminTrainingsMessages.NewTrainingDateInPast);
                }
                string trainingID = trainingManager.CreateNewTraining(dateTime, trainer, description, capacity);
                CreateTrainingModel model = new CreateTrainingModel();
                model.TrainingID = trainingID;
                model.Date = dateTime.ToString("dd.MM.yyyy");
                model.Time = dateTime.ToString("HH:mm");
                model.Description = description;
                model.Trainer = trainer;
                model.Message = AdminTrainingsMessages.NewTrainingSuccessfullyCreated;
                model.Capacity = capacity;
                return Json(model);
            }
            catch (InvalidDateException)
            {
                return Json(AdminTrainingsMessages.NewTrainingDateInvalid);
            }
            catch (InvalidTimeException)
            {
                return Json(AdminTrainingsMessages.NewTrainingTimeInvalid);
            }
        }

        public ActionResult CancelTraining(string trainingID)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(trainingID))
            {
                return Json(AdminTrainingsMessages.TrainingIDInvalid);
            }
            trainingManager.CancelTraining(trainingID);
            return Json(AdminTrainingsMessages.TrainingWasCanceled);
        }

        public ActionResult SetSignOffLimit(int limit)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            trainingManager.SetSignOffLimit(limit);
            return Json(AdminTrainingsMessages.SignOffLimitWasChanged);
        }

        public ActionResult AddUserToTraining(string trainingID,string userData)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (userData == null)
            {
                return Json(CalendarMessages.UserDoesNotExist);
            }

            string email = parser.ParseData(userData);

            if(email == null)
            {
                return Json(CalendarMessages.UserDoesNotExist);
            }
            var message = trainingManager.AddUserToTraining(email, trainingID, dateTimeService.GetCurrentDateTime(),true);
            if(message == CalendarMessages.SignUpSuccessful)
            {
                return Json(userManager.GetAddedUserData(email));
            }
            return Json(message);
        }

        [HttpPost]
        public ActionResult CreateRecurring(RecurringModel model)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            RecurringModel result = new RecurringModel();

            if(model == null)
            {
                Session["Message"] = RecurringTrainingMessages.NoModel;
                return RedirectToAction("Recurring", "AdminFutureTrainings");
            }
            if(string.IsNullOrEmpty(model.Trainer))
            {
                Session["Message"] = RecurringTrainingMessages.NoTrainer;
                return RedirectToAction("Recurring", "AdminFutureTrainings");
            }
            if (string.IsNullOrEmpty(model.Description))
            {
                Session["Message"] = RecurringTrainingMessages.NoDescription;
                return RedirectToAction("Recurring", "AdminFutureTrainings");
            }
            if (model.Capacity <= 0)
            {
                Session["Message"] = RecurringTrainingMessages.NoCapacity;
                return RedirectToAction("Recurring", "AdminFutureTrainings");
            }
            trainingManager.CreateRecurringTraining(model);
            Session["Message"] = RecurringTrainingMessages.Success;
            return RedirectToAction("Recurring", "AdminFutureTrainings");

        }

        public ActionResult Recurring()
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }

            RecurringModel model = new RecurringModel();
            IList<IRecurringTemplateModel> templates = trainingManager.GetTemplates();
            foreach (var item in templates)
            {
                int index = item.Day *15 + item.Time - 6;
                model.IsTrainingInTime[index] = true;
                model.Templates.Add(item);
            }
            if (Session["Message"] != null)
            {
                model.Message =(RecurringTrainingMessages) Session["Message"];
            }
            Session["Message"] = null;
            return View(model);
        }

        public ActionResult RemoveTemplate(int day, int time)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            trainingManager.RemoveTrainingTemplate(day, time);
            return Json("");
        }

        public ActionResult ShowTrainingsPage(DateTime date)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            TrainingsPageModel model = new TrainingsPageModel();
            model.Trainings = trainingManager.GetFutureTrainings(date);
            return PartialView("TrainingsPage",model);
        }

    }
}