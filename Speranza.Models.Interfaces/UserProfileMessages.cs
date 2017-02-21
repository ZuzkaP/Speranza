using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models.Interfaces
{
    public enum UserProfileMessages
    {
        NoMessage = 0,
        PassAndHashAreNotTheSame = 9,
        NewPassIsEmpty = 1,
        ConfirmPassIsEmpty = 2,
        NewPassIsTooShort = 3,
        NewPassHasNoNumber = 4,
        NewPassHasNoLetter = 5,
        NewPassAndConfirmPassAreNotTheSame = 6,
        PassWasSucessfullyChanged = 7,
        ProfileWasUpdated = 8,
        SurnameIsEmpty = 10,
        NameIsEmpty = 11
  
}
}
