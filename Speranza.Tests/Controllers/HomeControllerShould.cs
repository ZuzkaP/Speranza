using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Controllers;
using System.Web.Mvc;

namespace Speranza.Tests.Controllers
{
    [TestClass]
    public class HomeControllerShould
    {
        private HomeController controller;

        [TestMethod]
        public void OpenIndexHtml()
        {
            InitializeController();

            ViewResult result = controller.Index();

            Assert.AreEqual("Index", result.ViewName);

        }

        private void InitializeController()
        {
            controller = new HomeController();
        }
    }
}
