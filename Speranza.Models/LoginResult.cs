using Speranza.Common.Data;
using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
   public class LoginResult : ILoginResult
    {
        public bool IsAdmin { get; set; }
        public UserCategories Category { get; set; }
        public string Email { get; set; }
    }
}
