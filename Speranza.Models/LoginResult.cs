using Speranza.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
   public class LoginResult
    {
        public bool IsAdmin { get; set; }
        public UserCategories Category { get; set; }
    }
}
