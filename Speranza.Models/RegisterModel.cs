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
        UserAlreadyExists = 8,
        EmailFormatIsIncorrect = 16,
        PasswordIsTooShort = 32,
        PasswordHasNoNumber = 64,
        PasswordHasNoLetter = 128
    }

    public class RegisterModel
    {
       
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