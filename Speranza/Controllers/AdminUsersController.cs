using Speranza.App_Start;
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

            if(isAdmin)
            {
                return Json(UsersAdminMessages.SuccessfullySetAdminRole);
            }
            else
            {
                return Json(UsersAdminMessages.SuccessfullyClearAdminRole);
            }
        }
    }
}