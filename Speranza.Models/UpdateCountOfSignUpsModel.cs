using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class UpdateCountOfSignUpsModel
    {
        public int AfterChangeNumberOfSignUps { get; set; }
        public string Email { get; set; }
        public int ChangeNumberOfSignUps { get; set; }
        public UsersAdminMessages Message { get; set; }
    }
}
