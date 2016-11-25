﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Models
{
    public class User : IUser
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public User(string email, string password)
        {
            Email = email;
            PasswordHash = password;
        }
    }
}
