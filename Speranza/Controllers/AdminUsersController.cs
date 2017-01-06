﻿using Speranza.App_Start;
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

namespace Speranza.Controllers
{
    public class AdminUsersController : Controller
    {
        IUserManager userManager;

        public AdminUsersController(): this(Initializer.UserManager)
        {

        }

        public AdminUsersController( IUserManager userManager)
        {
            this.userManager = userManager;
        }
        // GET: AdminUsers
        public ActionResult AdminUsers()
        {
            if(userManager.IsUserLoggedIn(Session))
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
                model.Message = UsersAdminMessages.SuccessfullySetAdminRole;
            }
            else
            {
                model.Message = UsersAdminMessages.SuccessfullyClearAdminRole;
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
            model.Message = UsersAdminMessages.SuccessfullyChangedCategory;
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
                model.Message = UsersAdminMessages.SuccessfullyIncreasedCountOfSignUps;
            }
            else
            {
                model.ChangeNumberOfSignUps = countUpdate*(-1);
                model.Message = UsersAdminMessages.SuccessfullyDecreasedCountOfSignUps;
            }
            return Json(model);
        }

        public ActionResult TrainingsDetails(string id)
        {
            return Json("");
        }

    }
}