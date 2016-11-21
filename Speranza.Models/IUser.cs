namespace Speranza.Models
{
    public interface IUser
    {
        string PasswordHash { get; }
    }
}