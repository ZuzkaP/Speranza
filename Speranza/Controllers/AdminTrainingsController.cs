using Speranza.App_Start;
using Speranza.Database;
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
    public class AdminTrainingsController : Controller
    {
        private ITrainingsManager trainingManager;
        IUserManager userManager;

        public AdminTrainingsController(): this(Initializer.UserManager,Initializer.TrainingsManager)
        {

        }

        public AdminTrainingsController(IUserManager userManager,ITrainingsManager trainingManager)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
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
                return View("AdminTrainings",model);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangeTrainer(string trainingID, string trainer)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if(string.IsNullOrEmpty(trainer) || string.IsNullOrEmpty(trainingID))
            {
                return Json("");
            }
            trainingManager.SetTrainer(trainingID, trainer);
            
            return Json(AdminTrainingsMessages.TrainerWasSuccessfullyChanged);
        }
    }
}