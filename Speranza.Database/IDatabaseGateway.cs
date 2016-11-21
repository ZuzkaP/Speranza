using Speranza.Models;

namespace Speranza.Database
{
    public interface IDatabaseGateway
    {
        void RegisterNewUser(RegisterModel model);
        bool UserExists(string email);
        IUser LoadUser(LoginModel loginModel);
    }
}