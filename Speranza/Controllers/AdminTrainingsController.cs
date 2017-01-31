﻿using Speranza.App_Start;
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
        private IUserManager userManager;
        private IDateTimeService dateTimeService;
        private IUserDataParser parser;

        public AdminTrainingsController() : this(Initializer.UserManager, Initializer.TrainingsManager, Initializer.DateTimeService, Initializer.UserDataParser)
        {

        }

        public AdminTrainingsController(IUserManager userManager, ITrainingsManager trainingManager, IDateTimeService dateTimeService, IUserDataParser parser)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.dateTimeService = dateTimeService;
            this.parser = parser;
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
                IList<ITrainingForAdminModel> trainings = trainingManager.GetAllFutureTrainings();
                IList<IUserForTrainingDetailModel> users = userManager.GetAllUsersForTrainingDetails();
                int signOffLimit = trainingManager.GetSignOffLimit();
                AdminTrainingsModel model = new AdminTrainingsModel();
                model.Trainings = trainings;
                model.Users = users;
                model.SignOffLimit = signOffLimit;
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
            model.Users = users;

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
                if(dateTime < dateTimeService.GetCurrentDate())
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
            var message = trainingManager.AddUserToTraining(email, trainingID, dateTimeService.GetCurrentDate());
            if(message == CalendarMessages.SignUpSuccessful)
            {
                return Json(userManager.GetAddedUserData(email));
            }
            return Json(message);
        }

    }
}