using System;
using System.Web.Mvc;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Common.Data;
using Speranza.Controllers;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class GalleryControllerShould
    {
        private Mock<IUserManager> userManager;
        private GalleryController gallery;
        private Mock<ICookieService> cookieService;

        [TestMethod]
        public void ReturnToLogin_When_UserIsNotLoggedIn()
        {
            InitializeController();
            userManager.Setup(r => r.IsUserLoggedIn(null, gallery.Session)).Returns(false);
            ActionResult result = gallery.Index();

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));

            Assert.AreEqual("Home", ((RedirectToRouteResult)result).RouteValues["controller"]);
            Assert.AreEqual("Index", ((RedirectToRouteResult)result).RouteValues["action"]);
        }


        [TestMethod]
        public void ReturnEmptyGalleryWhenNoFolderExists()
        {
            Assert.Fail("to do!!");
        }

        


        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            cookieService = new Mock<ICookieService>();
            gallery = new GalleryController(userManager.Object,cookieService.Object);

            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            gallery.ControllerContext = new FakeControllerContext(gallery, sessionItems);
        }
    }
}
