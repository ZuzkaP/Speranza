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
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages;

namespace Speranza.Controllers
{
    public class AdminUsersController : Controller
    {
        IUserManager userManager;
        private ITrainingsManager trainingManager;
        private IMessageManager messageManager;
        private ICookieService cookieService;
        private IDateTimeService dateTimeService;
        private const int ALLOWED_MESSAGE_LENGHT = 100;

        public AdminUsersController(): this(Initializer.UserManager,Initializer.TrainingsManager, Initializer.CookieService, Initializer.DateTimeService, Initializer.MessageManager)
        {

        }

        public AdminUsersController( IUserManager userManager, ITrainingsManager trainingManager, ICookieService cookieService, IDateTimeService dateTimeService, IMessageManager messageManager)
        {
            this.userManager = userManager;
            this.trainingManager = trainingManager;
            this.cookieService = cookieService;
            this.dateTimeService = dateTimeService;
            this.messageManager = messageManager;
        }
        // GET: AdminUsers
        public ActionResult AdminUsers()
        {
            if(userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies),Session))
            {
                if (!userManager.IsUserAdmin(Session))
                {
                    return RedirectToAction("Calendar", "Calendar");
                }

                IList<IUserForAdminModel> users = userManager.GetAllUsersForAdmin();
                AdminUsersModel model = new AdminUsersModel();
                model.Users = users;
                model.Categories.Add(UserCategories.Standard.ToString());
                model.Categories.Add(UserCategories.Silver.ToString());
                model.Categories.Add(UserCategories.Gold.ToString());

                return View("AdminUsers",model);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult ToggleAdmin(string id,bool isAdmin)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(id))
            {
                return Json(string.Empty);
            }

            userManager.SetUserRoleToAdmin(id, isAdmin);
            ToggleAdminModel model = new ToggleAdminModel();
            model.Email = id;

            if(isAdmin)
            {
                model.Message = AdminUsersMessages.SuccessfullySetAdminRole;
            }
            else
            {
                model.Message = AdminUsersMessages.SuccessfullyClearAdminRole;
            }
            return Json(model);
        }

        public ActionResult UserCategory(string id, string category)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(category))
            {
                return Json(string.Empty);
            }
            UserCategories categoryToDb;
            bool resultOfCategoryParse = Enum.TryParse<UserCategories>(category,out categoryToDb);
            userManager.SetUserCategory(id, categoryToDb);

            UpdateCategoryModel model = new UpdateCategoryModel();
            model.Category = category;
            model.Email = id;
            model.Message = AdminUsersMessages.SuccessfullyChangedCategory;
            return Json(model);
        }

        public ActionResult UpdateSignUpCount(string id, int countUpdate)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(id))
            {
                return Json(string.Empty);
            }
            int afterChangeNumberOfSignUps = userManager.UpdateCountOfFreeSignUps(id, countUpdate);

            UpdateCountOfSignUpsModel model = new UpdateCountOfSignUpsModel();
            model.Email = id;
            model.AfterChangeNumberOfSignUps = afterChangeNumberOfSignUps;

            if(countUpdate > 0)
            {
                model.ChangeNumberOfSignUps = countUpdate;
                model.Message = AdminUsersMessages.SuccessfullyIncreasedCountOfSignUps;
            }
            else
            {
                model.ChangeNumberOfSignUps = countUpdate*(-1);
                model.Message = AdminUsersMessages.SuccessfullyDecreasedCountOfSignUps;
            }
            return Json(model);
        }

        public ActionResult TrainingsDetails(string id)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(id))
            {
                return Json(string.Empty);
            }
            IList<ITrainingModel> trainings = userManager.GetFutureTrainingsForUser(id);
            TrainingsDetailsModel model = new TrainingsDetailsModel();
            model.Trainings = trainings;
            model.Email = id;

            return PartialView("UserTrainingsDetails",model);
        }

        public ActionResult SignOutFromTraining(string id, string training)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(training))
            {
                return Json(string.Empty);
            }
            ITrainingModel trainingModel = trainingManager.RemoveUserFromTraining(id, training, true);
            UserSignOffModel model = new UserSignOffModel();
            model.Email = id;
            model.TrainingDate = trainingModel.Time.ToString("dd.MM.yyyy");
            model.TrainingTime = trainingModel.Time.ToString("HH:mm");
            model.Message = AdminUsersMessages.SuccessfullyUserSignOffFromTraining;
            return Json(model);
        }

        public ActionResult AddNewMessage(string dateFrom, string dateTo, string message)
        {
            if (!userManager.IsUserAdmin(Session))
            {
                return RedirectToAction("Calendar", "Calendar");
            }
            DateTime dateTimeFrom = dateTimeService.ParseDate(dateFrom);
            DateTime dateTimeTo = dateTimeService.ParseDate(dateTo);
            if (dateTimeFrom.Date < dateTimeService.GetCurrentDate() || dateTimeTo.Date < dateTimeService.GetCurrentDate())
            {
                return Json(AdminUsersInfoMessage.MessageInPast);
            }

            var model = new UserNotificationMessageModel();
            model.Message = message;
            if (model.Message.Length > ALLOWED_MESSAGE_LENGHT)
            {
                return Json(AdminUsersInfoMessage.MessageIsTooLong);

            }
            model.Message = message;
            model.DateFrom = dateTimeFrom;
            model.DateTo = dateTimeTo;
            model.Status = AdminUsersInfoMessage.MessageSuccessfullyAdded;
            messageManager.AddNewInfoMessage(dateTimeFrom, dateTimeTo, message);
            return Json(model);
        }

        public ViewResult GetUsersInfoMessage()
        {
            IUserNotificationMessage infoMessage = messageManager.GetMessageForCurrentDate();
            if (infoMessage == null)
            {
                return null;
            }
            else
            {
                IUserNotificationMessageModel model = new UserNotificationMessageModel();
                model.Message = infoMessage.Message;
                model.DateFrom = infoMessage.DateFrom;
                model.DateTo = infoMessage.DateTo;
                return View(model);
            }
        }
    }
}