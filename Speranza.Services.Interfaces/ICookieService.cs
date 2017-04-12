using System.Web;

namespace Speranza.Services.Interfaces
{
    public interface ICookieService
    {
        void SetRememberMeCookie(HttpCookieCollection cookies,string series, string token);
    }
}