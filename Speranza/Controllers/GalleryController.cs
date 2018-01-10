using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Speranza.App_Start;
using Speranza.Models;
using Speranza.Services.Interfaces;

namespace Speranza.Controllers
{
    public class GalleryController : Controller
    {
        private ICookieService cookieService;

        private IUserManager userManager;
        private IGalleryService galleryService;

        public GalleryController() : this(Initializer.UserManager,Initializer.CookieService,Initializer.GalleryService)
        {
                
        }

        public GalleryController(IUserManager userManager,ICookieService cookieService,IGalleryService galleryService)
        {
            this.cookieService = cookieService;
            this.userManager = userManager;
            this.galleryService = galleryService;
        }

        // GET: Gallery
        public ActionResult Index()
        {
            if (!userManager.IsUserLoggedIn(cookieService.GetRememberMeCookie(Request.Cookies), Session))
            {
                return RedirectToAction("Index", "Home");
            }
            var folders = galleryService.GetFoldersWithImages();

            var model = new GalleryModel();
            if (folders != null)
            {
                foreach (var folder in folders)
                {
                    var photosInFolder = galleryService.GetPhotosFromFolder(folder);
                    foreach (var photo in photosInFolder)
                    {
                        PhotoModel photoModel = new PhotoModel();
                        photoModel.Source = photo;
                        photoModel.Tag = galleryService.ConvertFolderPathToTag(folder);
                        model.Photos.Add(photoModel);
                    }
                }
            }
            return View(model);
        }
    }
}