namespace Speranza.Models
{
    public enum UserCategories
    {
        Standard,
        Silver,
        Gold
    }

    public interface IUser
    {
        string PasswordHash { get; }
    }
}