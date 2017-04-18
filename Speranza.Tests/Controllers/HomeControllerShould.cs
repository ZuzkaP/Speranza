using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using System.Web.Mvc;
using Speranza.Services.Interfaces;
using Moq;
using Speranza.Common.Data;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using System.Web.SessionState;
using System.Web;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class HomeControllerShould
    {
        private const string COOKIE = "cookie";
        private HomeController controller;
        private Mock<IUserManager> userManager;
        private Mock<ICookieService> cookieService;
        private const UserCategories CATEGORY = UserCategories.Gold;

        public const string EMAIL = "email";
        private Mock<ILoginResult> user;

        [TestMethod]
        public void OpenIndexHtml()
        {
            InitializeController();

            ViewResult result = (ViewResult)controller.Index();

            Assert.AreEqual("Index", result.ViewName);

        }

        [TestMethod]
        public void DirectlyShowCalendar_When_RememberMeIsChecked()
        {
            InitializeController();
            PrepareCheckedRememberMe();

            ActionResult result = controller.Index();

            Assert.AreEqual(EMAIL, controller.Session["Email"]);
            Assert.AreEqual(CATEGORY, controller.Session["Category"]);
            Assert.IsTrue((bool)controller.Session["IsAdmin"]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Calendar", ((RedirectToRouteResult)result).RouteValues["action"]);
        }

        private void PrepareCheckedRememberMe()
        {
            cookieService.Setup(r => r.GetRememberMeCookie(controller.Request.Cookies)).Returns(COOKIE);
            user = new Mock<ILoginResult>();
            user.SetupGet(r => r.Email).Returns(EMAIL);
            user.SetupGet(r => r.Category).Returns(CATEGORY);
            user.SetupGet(r => r.IsAdmin).Returns(true);
            userManager.Setup(r => r.VerifyRememberMe(COOKIE)).Returns(user.Object);
        }

        private void InitializeController()
        {
           
            cookieService = new Mock<ICookieService>();
            userManager = new Mock<IUserManager>();
            controller = new HomeController(cookieService.Object,userManager.Object);
            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            HttpCookieCollection cookies = new HttpCookieCollection();

            controller.ControllerContext = new FakeControllerContext(controller, sessionItems, cookies);

        }
    }
}
