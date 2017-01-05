using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public enum UsersAdminMessages
    {
        NoMessage = 0,
        SuccessfullyClearAdminRole = 1,
        SuccessfullySetAdminRole = 2,
        SuccessfullyChangedCategory = 3
    }
    public class ToggleAdminModel
    {
        public UsersAdminMessages Message { get; set; }
        public string Email { get; set; }
    }
}
