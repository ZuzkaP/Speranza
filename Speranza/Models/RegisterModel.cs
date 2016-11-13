using System;
using System.Collections.Generic;
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
        ConfirmPassIncorrect = 4
    }

    public class RegisterModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }

        public RegisterModelMessages Messages { get; set; }

       
    }
}