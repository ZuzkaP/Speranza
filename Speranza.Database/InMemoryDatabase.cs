using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models;

namespace Speranza.Database
{
    public class InMemoryDatabase : IDatabaseGateway
    { 

        Dictionary<string, RegisterModel> users;
        static InMemoryDatabase database;
        public static InMemoryDatabase Instance
        {
            get
            {
                if (database == null)
                    database = new InMemoryDatabase();
                return database;
            }
        }

        private InMemoryDatabase()
        {
            users = new Dictionary<string, RegisterModel>();
        }

        public void RegisterNewUser(RegisterModel model)
        {
            users.Add(model.Email, model);
        }

        public bool UserExists(string email)
        {
            return users.ContainsKey(email);
        }

        public void LoadUser(LoginModel loginModel)
        {
            throw new NotImplementedException();
        }
    }
}
