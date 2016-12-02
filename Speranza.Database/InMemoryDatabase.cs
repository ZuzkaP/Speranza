using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models;
using Speranza.Database.Data.Interfaces;
using Speranza.Database.Data;

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
            users.Add("admin", new RegisterModel() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==" });
        }

        public void RegisterNewUser(RegisterModel model)
        {
            users.Add(model.Email, model);
        }

        public bool UserExists(string email)
        {
            return users.ContainsKey(email);
        }

        public IUser LoadUser(string email)
        {
            if(users.ContainsKey(email))
            {
                IUser user = new User(email, users[email].Password);
                return user;
            }

            return null;
        }

        public IList<ITraining> GetDayTrainings(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
