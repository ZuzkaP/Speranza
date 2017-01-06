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
        string Email { get; }
        string Name { get; }
        string Surname { get; }
        string PhoneNumber { get; }
        bool IsAdmin { get; }
        UserCategories Category { get; set; }
        int NumberOfFreeSignUpsOnSeasonTicket { get; set; }
        int NumberOfPastTrainings { get; set; }
        int NumberOfSignedUpTrainings { get; set; }
    }
}