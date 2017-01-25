using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{

    public enum UserProfileMessages
    {
        PassAndHashAreNotTheSame,
        NewPassIsEmpty,
        ConfirmPassIsEMpty,
        NewPassIsTooShort,
        NewPassHasNoNumber,
        NewPassHasNoLetter,
        NewPassAndConfirmPassAreNotTheSame,
        PassWasSucessfullyChanged
    }
     public class ChangePassModel
    {
        public string OldPass { get; set; }
        public string NewPass { get; set; }
        public string ConfirmPass { get; set; }
        public string Email { get; set; }
        public UserProfileMessages Message { get; set; }
    }
}
