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
        List<ITraining> trainings;
        List<UserInTraining> usersInTrainings;
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
            usersInTrainings = new List<UserInTraining>();
            users.Add("admin", new RegisterModel() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "Zuzana", Surname = "Papalova", PhoneNumber = "1234" });
            trainings = new List<ITraining>();

            trainings.Add(PrepareTraining(new DateTime(2016, 12, 12, 12, 00, 00), "training c.1", "Zuzka", 10, 10));
            trainings.Add(PrepareTraining(new DateTime(2016, 12, 12, 13, 00, 00), "training c.2", "Dano", 10, 6));
            trainings.Add(PrepareTraining(new DateTime(2016, 12, 15, 08, 00, 00), "training c.3", "Filip", 10, 6));
            usersInTrainings.Add(new UserInTraining() { Email = "admin", TrainingID = trainings[0].ID });
        }

        private ITraining PrepareTraining(DateTime dateTime, string v1, string v2, int v3, int v4)
        {

            return new Training(Guid.NewGuid().ToString(),dateTime, v1, v2, v3, v4);
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
            if (users.ContainsKey(email))
            {
                IUser user = new User(email, users[email].Password);
                return user;
            }

            return null;
        }

        public IList<ITraining> GetDayTrainings(DateTime date)
        {
            return trainings.Where(r => r.Time.Date == date.Date).ToList();
        }

        public IUser GetUserData(string email)
        {
            if (users.ContainsKey(email))
            {
                IUser user = new User(email, users[email].Name, users[email].Surname, users[email].PhoneNumber);
                return user;
            }

            return null;
        }

        public void UpdateUserData(UserProfileModel userProfileModel)
        {
            if (users.ContainsKey(userProfileModel.Email))
            {
                users[userProfileModel.Email].Name = userProfileModel.Name;
                users[userProfileModel.Email].Surname = userProfileModel.Surname;
                users[userProfileModel.Email].PhoneNumber = userProfileModel.PhoneNumber;
            }
        }

        public ITraining GetTrainingData(string trainingID)
        {
            return trainings.FirstOrDefault(r => r.ID == trainingID);
        }

        public void AddUserToTraining(string email, string trainingID)
        {
            usersInTrainings.Add(new UserInTraining() { Email = email, TrainingID = trainingID });
        }

        private class UserInTraining
        {
          public  string Email { get; set; }
          public string TrainingID { get; set; }
        }
    }

}