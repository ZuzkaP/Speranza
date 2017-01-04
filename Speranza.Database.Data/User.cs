using Speranza.Database.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Database.Data
{
    public class User : IUser
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
        

        public string PhoneNumber { get; set; }

        public bool IsAdmin {  get; set; }

        public User(string email, string password, bool isAdmin)
        {
            Email = email;
            PasswordHash = password;
            IsAdmin = isAdmin;
        }

        public User(string email, string name, string surname, string phoneNumber, bool isAdmin)
        {
            Email = email;
            Name = name;
            Surname = surname;
            PhoneNumber = phoneNumber;
            IsAdmin = isAdmin;
        }
    }
}
