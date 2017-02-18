using Speranza.Common.Data;

namespace Speranza.Models.Interfaces
{
    public interface ILoginResult
    {
        UserCategories Category { get; set; }
        string Email { get; set; }
        bool IsAdmin { get; set; }
    }
}