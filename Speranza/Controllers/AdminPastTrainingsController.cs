using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Speranza.Services.Interfaces;
using Speranza.App_Start;
using Speranza.Models.Interfaces;
using Speranza.Models;

namespace Speranza.Controllers
{
    public class AdminPastTrainingsController : Controller
    {
        private IUserManager userManager;
        private ITrainingsManager trainingManager;
        private IDateTimeService dateTimeService;
        private const int DEFAULT_PAGE_SIZE = 20;

        public AdminPastTrainingsController(IUserManager userManager, ITrainingsManager trainingManager, IDateTimeService dateTimeService)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
        }

        public AdminPastTrainingsController() : this(Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService)
        {

        }

        // GET: AdminPastTrainings
        public ActionResult AdminTrainings(int? pageSize = null)
        {
            if (userManager.IsUserLoggedIn(Session))
            {
                if (!userManager.IsUserAdmin(Session))
                {
                    return RedirectToAction("Calendar", "Calendar");
                }
                var setPageSize = pageSize.HasValue ? pageSize.Value : DEFAULT_PAGE_SIZE;
                int numberOfPages = (int)Math.Ceiling(trainingManager.GetPastTrainingsCount() / (double)setPageSize);
                IList<ITrainingForAdminModel> trainings = trainingManager.GetPastTrainings(0, setPageSize);
                AdminTrainingsModel model = new AdminTrainingsModel();
                model.PageSize = setPageSize;
                model.PageNumber = 1;
                model.Trainings = trainings;
                model.PagesCount = numberOfPages;
                return View("AdminTrainings",model);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ShowTrainingsPage(int page, int size)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            TrainingsPageModel model = new TrainingsPageModel();
            model.Trainings = trainingManager.GetPastTrainings(size * (page - 1), size * page);
            return PartialView("TrainingsPage", model);
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
            model.Users = users;

            return PartialView("UsersInTraining", model);
        }

        public ActionResult ConfirmParticipation(string trainingID, string email)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if(string.IsNullOrEmpty(trainingID) || string.IsNullOrEmpty(email))
            {
                return Json("");
            }
            trainingManager.ConfirmParticipation(trainingID, email);

            return Json(AdminTrainingsMessages.ParticipationConfirmed);
        }

        public ActionResult DisproveParticipation(string trainingID, string email)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(trainingID) || string.IsNullOrEmpty(email))
            {
                return Json("");
            }
            trainingManager.DisproveParticipation(trainingID, email);

            return Json(AdminTrainingsMessages.ParticipationDisproved);
        }
    }
}