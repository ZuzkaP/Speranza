using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Web;

namespace Speranza.Models
{
    
    public class AdminUsersModel 
    {
        public IList<string> Categories { get;private set; }
        public IList<IUserForAdminModel> Users { get; set; }

        public AdminUsersModel()
        {
            Categories = new List<string>();
        }
    }
}
