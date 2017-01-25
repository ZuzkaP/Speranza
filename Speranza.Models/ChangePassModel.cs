using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{

    public enum UserProfileMessages
    {
        PassAndHashAreNotTheSame
    }
     public class ChangePassModel
    {
        public string oldPass { get; set; }
        public string newPass { get; set; }
        public string ConfirmPass { get; set; }
        public string Email { get; set; }
    }
}
