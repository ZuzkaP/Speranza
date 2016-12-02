namespace Speranza.Database.Data.Interfaces
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