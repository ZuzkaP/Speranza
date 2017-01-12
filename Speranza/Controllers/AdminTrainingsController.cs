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
    public class AdminTrainingsController : Controller
    {
        private ITrainingsManager trainingManager;
        IUserManager userManager;
        private IDateTimeService dateTimeService;

        public AdminTrainingsController() : this(Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService)
        {

        }

        public AdminTrainingsController(IUserManager userManager, ITrainingsManager trainingManager, IDateTimeService dateTimeService)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
        }

        // GET: AdminTrainings
        public ActionResult AdminTrainings()
        {
            if (userManager.IsUserLoggedIn(Session))
            {
                if (!userManager.IsUserAdmin(Session))
                {
                    return RedirectToAction("Calendar", "Calendar");
                }

                IList<ITrainingForAdminModel> trainings = trainingManager.GetAllTrainingsForAdmin();
                AdminTrainingsModel model = new AdminTrainingsModel();
                model.Trainings = trainings;
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
            if (capacity < 0)
            {
                return Json(AdminTrainingsMessages.TraininingCapacityCannotBeLessThanZero);
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
            model.Users = users;

            return PartialView("UsersInTraining", model);
        }

        public ActionResult CreateNewTraining(string date, string time, string trainer, string description)
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
            try
            {
                DateTime dateTime = dateTimeService.ParseDateTime(date, time);
                string trainingID = trainingManager.CreateNewTraining(dateTime, trainer, description);
                CreateTrainingModel model = new CreateTrainingModel();
                model.TrainingID = trainingID;
                model.Date = dateTime;
                model.Description = description;
                model.Trainer = trainer;
                model.Message = AdminTrainingsMessages.NewTrainingSuccessfullyCreated;
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
                throw new IInvalidTrainingIDException();
            }
            return Json("");
        }

    }
}