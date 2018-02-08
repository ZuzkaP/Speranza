using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Common.Data;
using Speranza.Controllers;
using Speranza.Models;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class GalleryControllerShould
    {
        private const string FOLDERA = "foldera";
        private const string FOLDERB = "folderb";
        private Mock<IUserManager> userManager;
        private GalleryController gallery;
        private Mock<ICookieService> cookieService;
        private Mock<IGalleryService> galleryService;
        private const string PHOTO_A = "photoA";
        private const string PHOTO_B = "photoB";
        private const string PHOTO_C = "photoC";
        private const string TAG_A = "TagA";
        private const string TAG_B = "TagB";

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
            InitializeController();
            galleryService.Setup(r => r.GetFoldersWithImages()).Returns((List<string>) null);
            UserIsLoggedIn();

            ViewResult result = (ViewResult)gallery.Index();

            var galleryModel =(GalleryModel) result.Model;
            Assert.IsNotNull(galleryModel);
            Assert.AreEqual(0,galleryModel.Photos.Count);
        }

        [TestMethod]
        public void ReturnEmptyGalleryWhenNoPhotosInFoldersExists()
        {
            InitializeController();
            var folders = new List<string>() { FOLDERA, FOLDERB};
            galleryService.Setup(r => r.GetFoldersWithImages()).Returns(folders);
            galleryService.Setup(r => r.GetPhotosFromFolder(folders[0])).Returns(new List<string>());
            galleryService.Setup(r => r.GetPhotosFromFolder(folders[1])).Returns(new List<string>());
            UserIsLoggedIn();

            ViewResult result = (ViewResult)gallery.Index();

            var galleryModel = (GalleryModel)result.Model;
            Assert.IsNotNull(galleryModel);
            Assert.AreEqual(0, galleryModel.Photos.Count);
        }

        [TestMethod]
        public void ReturnGalleryWithPhotosInFoldersWhenExist()
        {
            InitializeController();
            var folders = new List<string>() { FOLDERA, FOLDERB };
            galleryService.Setup(r => r.GetFoldersWithImages()).Returns(folders);
            galleryService.Setup(r => r.GetPhotosFromFolder(folders[0])).Returns(new List<string>(){PHOTO_A,PHOTO_B});
            galleryService.Setup(r => r.GetPhotosFromFolder(folders[1])).Returns(new List<string>(){PHOTO_C});
            galleryService.Setup(r => r.ConvertFolderPathToTag(FOLDERA)).Returns(TAG_A);
            galleryService.Setup(r => r.ConvertFolderPathToTag(FOLDERB)).Returns(TAG_B);
            UserIsLoggedIn();

            ViewResult result = (ViewResult)gallery.Index();

            var galleryModel = (GalleryModel)result.Model;
            Assert.IsNotNull(galleryModel);
            Assert.AreEqual(3, galleryModel.Photos.Count);
            Assert.AreEqual(TAG_A,galleryModel.Photos[0].Tag);
            Assert.AreEqual(TAG_A,galleryModel.Photos[1].Tag);
            Assert.AreEqual(TAG_B,galleryModel.Photos[2].Tag);
            Assert.AreEqual(PHOTO_A, galleryModel.Photos[0].Source);
            Assert.AreEqual(PHOTO_B, galleryModel.Photos[1].Source);
            Assert.AreEqual(PHOTO_C, galleryModel.Photos[2].Source);
        }



        private void InitializeController()
        {
            userManager = new Mock<IUserManager>();
            cookieService = new Mock<ICookieService>();
            galleryService = new Mock<IGalleryService>();
            gallery = new GalleryController(userManager.Object,cookieService.Object,galleryService.Object,null);

            SessionStateItemCollection sessionItems = new SessionStateItemCollection();
            gallery.ControllerContext = new FakeControllerContext(gallery, sessionItems);
        }

        private void UserIsLoggedIn()
        {
            userManager.Setup(r => r.IsUserLoggedIn(It.IsAny<string>(),It.IsAny<ICollection>())).Returns(true);
        }
    }

}
