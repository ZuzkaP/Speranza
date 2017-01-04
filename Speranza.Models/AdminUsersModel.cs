using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Web;

namespace Speranza.Models
{
    public enum UsersAdminMessages
    {
        SuccessfullySetAdminRole,
        SuccessfullyClearAdminRole
    }

    public class AdminUsersModel 
    {
        public IList<IUserForAdminModel> Users { get; set; }
   
    }
}
