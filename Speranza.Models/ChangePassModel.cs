using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{

    public enum UserProfileMessages
    {
        NoMessage = 0,
        PassAndHashAreNotTheSame = 9,
        NewPassIsEmpty = 1,
        ConfirmPassIsEmpty = 2,
        NewPassIsTooShort = 3,
        NewPassHasNoNumber = 4,
        NewPassHasNoLetter =5,
        NewPassAndConfirmPassAreNotTheSame = 6,
        PassWasSucessfullyChanged = 7,
        ProfileWasUpdated = 8
    }
     public class ChangePassModel : IChangePassModel
    {
       public string OldPass { get; set; }
       public string NewPass { get; set; }
       public string ConfirmPass { get; set; }
       public string Email { get; set; }
       public UserProfileMessages Message { get; set; }
    }
}
