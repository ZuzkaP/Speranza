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
        public void SetRememberMeCookie(HttpCookieCollection cookies,string series, string token)
        {
            var cookie = new HttpCookie("RememberMe");
            cookie.Values.Add(series, token);
            cookies.Add(cookie);
        }
    }
}
