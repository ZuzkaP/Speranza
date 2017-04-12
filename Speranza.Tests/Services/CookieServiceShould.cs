using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class CookieServiceShould
    {
        private const string TOKEN = "token";
        private const string SERIES = "cookie";
        private CookieService cookieService;
        private HttpCookieCollection cookiesCollection;

        [TestMethod]
        public void SetRememberMe()
        {
            InitializeCookieService();

            cookieService.SetRememberMeCookie(cookiesCollection,SERIES, TOKEN);

            Assert.AreEqual(TOKEN, cookiesCollection["RememberMe"][SERIES]);

        }

        private void InitializeCookieService()
        {
            cookiesCollection = new HttpCookieCollection();
            cookieService = new CookieService();
        }
    }
}
