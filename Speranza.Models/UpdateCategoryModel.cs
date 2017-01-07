using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class UpdateCategoryModel
    {
        public string Category { get; set; }
        public string Email { get; set; }
        public AdminUsersMessages Message { get; set; }
    }
}
