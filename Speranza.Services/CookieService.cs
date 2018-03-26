using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Speranza.Services
{
    public class CookieService : ICookieService
    {
        public string GetRememberMeCookie(HttpCookieCollection cookies)
        {
            if(cookies.AllKeys.Contains("RememberMe"))
            {
            return cookies["RememberMe"].Value;
            }
            return null;
        }

        public void SetRememberMeCookie(HttpCookieCollection cookies,string series, string token)
        {
            var cookie = new HttpCookie("RememberMe");
            cookie.Values.Add(series, token);
            cookie.Expires = DateTime.Now.AddMonths(1);
            cookies.Add(cookie);


        }
    }
}
