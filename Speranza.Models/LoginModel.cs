using System.ComponentModel.DataAnnotations;

namespace Speranza.Models
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        public bool LoginSuccessful { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}