using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Speranza.Models
{
    [Flags]
    public enum RegisterModelMessages
    {
        NoMessage = 0,
        EmailIsEmpty = 1,
        PasswordIsEmpty = 2,
        ConfirmPassIncorrect = 4,
        UserAlreadyExists = 8
    }

    public class RegisterModel
    {
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail je v nesprávnom tvare")]
        [Required]
        public string Email { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public RegisterModelMessages Messages { get; set; }

       
    }
}