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

        Dictionary<string, RegisteredUser> users;
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
            users = new Dictionary<string, RegisteredUser>();
            usersInTrainings = new List<UserInTraining>();
            users.Add("admin", new RegisteredUser() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "Zuzana", Surname = "Papalova", PhoneNumber = "1234" , IsAdmin = true, Category = UserCategories.Silver});
            
            users.Add("miro", new RegisteredUser() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "Miro", Surname = "Pavlicko", PhoneNumber = "1234" , IsAdmin = false , NumberOfFreeSignUps = 10});
            trainings = new List<ITraining>();

            trainings.Add(PrepareTraining(new DateTime(2017, 1, 15, 12, 00, 00), "training c.1", "Zuzka", 10 ));
            trainings.Add(PrepareTraining(new DateTime(2017, 1, 10, 14, 00, 00), "training c.2", "Dano", 10));
            trainings.Add(PrepareTraining(new DateTime(2017, 1, 10, 08, 00, 00), "training c.4", "Filip", 10 ));
            trainings.Add(PrepareTraining(new DateTime(2017, 1, 19, 14, 00, 00), "training c.5", "Filip", 10 ));
            trainings.Add(PrepareTraining(new DateTime(2017, 1, 13, 09, 00, 00), "training c.3", "Filip", 10 ));
            trainings.Add(PrepareTraining(new DateTime(2016, 12,18, 09, 00, 00), "training c.3", "Filip", 10 ));
            usersInTrainings.Add(new UserInTraining() { Email = "admin", TrainingID = trainings[0].ID });
            usersInTrainings.Add(new UserInTraining() { Email = "admin", TrainingID = trainings[5].ID });
        }

        private ITraining PrepareTraining(DateTime dateTime, string v1, string v2, int v3)
        {

            return new Training(Guid.NewGuid().ToString(),dateTime, v1, v2, v3,0);
        }

        public void RegisterNewUser(RegisterModel model)
        {
            RegisteredUser user = new RegisteredUser();
            user.Name =model.Name;
            user.Email =model.Email;
            user.IsAdmin =false;
            user.Password =model.Password;
            user.PhoneNumber =model.PhoneNumber;
            user.Surname =model.Surname;

            users.Add(model.Email, user);
        }

        public bool UserExists(string email)
        {
            return users.ContainsKey(email);
        }

        public IUser LoadUser(string email)
        {
            if (users.ContainsKey(email))
            {
                User user = new User();
                user.Email = email;
                user.Category = users[email].Category;
                user.IsAdmin = users[email].IsAdmin;
                user.PasswordHash = users[email].Password;

                return user;
            }

            return null;
        }

        public IList<ITraining> GetDayTrainings(DateTime date)
        {
            var t =  trainings.Where(r => r.Time.Date == date.Date).ToList();
            foreach (var item in t)
            {
                item.RegisteredNumber = usersInTrainings.Count(r=>r.TrainingID == item.ID);
            }
            return t;

        }

        public IUser GetUserData(string email)
        {
            if (users.ContainsKey(email))
            {
                User user = new User();
                user.Email = email;
                user.Category = users[email].Category;
                user.IsAdmin = users[email].IsAdmin;
                user.Name = users[email].Name;
                user.Surname = users[email].Surname;
                user.PhoneNumber = users[email].PhoneNumber;
                user.NumberOfFreeSignUpsOnSeasonTicket = users[email].NumberOfFreeSignUps;
                user.NumberOfPastTrainings = usersInTrainings.Count(r=> r.Email == email && trainings.First(p=>p.ID == r.TrainingID).Time < DateTime.Now);
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
            var t = trainings.FirstOrDefault(r => r.ID == trainingID);
            if(t != null)
            {
                t.RegisteredNumber = usersInTrainings.Count(r => r.TrainingID == t.ID);
            }
            return t ;
        }

        public void AddUserToTraining(string email, string trainingID, DateTime time)
        {
            usersInTrainings.Add(new UserInTraining() { Email = email, TrainingID = trainingID, Time = time});
        }

        public bool IsUserAlreadySignedUpInTraining(string email, string trainingID)
        {
            return usersInTrainings.Any(r => r.Email == email && r.TrainingID == trainingID);
        }

        public void RemoveUserFromTraining(string email, string id)
        {
            UserInTraining toBeRemoved = usersInTrainings.FirstOrDefault(r => r.Email == email && r.TrainingID == id);
            if(toBeRemoved != null)
            {
                usersInTrainings.Remove(toBeRemoved);
            }
        }

        public IList<ITraining> GetTrainingsForUser(string email)
        {
            List<UserInTraining> trainingsForUser = usersInTrainings.Where(r => r.Email == email).ToList();
            List<ITraining> selectedTrainings = new List<ITraining>();
            foreach (var item in trainingsForUser)
            {
                selectedTrainings.Add(trainings.First(r => r.ID == item.TrainingID));
            }
            return selectedTrainings;
        }

        public IList<IUser> GetAllUsers()
        {
            var allusers = new List<IUser>();
            foreach (var item in users)
            {
                string email = item.Key;

                User user = new User();
                user.Email = email;
                user.Category = users[email].Category;
                user.IsAdmin = users[email].IsAdmin;
                user.Name = users[email].Name;
                user.Surname = users[email].Surname;
                user.PhoneNumber = users[email].PhoneNumber;
                user.NumberOfFreeSignUpsOnSeasonTicket = users[email].NumberOfFreeSignUps;
                user.NumberOfSignedUpTrainings = usersInTrainings.Count(r => r.Email == email && trainings.First(p => p.ID == r.TrainingID).Time > DateTime.Now);
                
                allusers.Add(user);
            }
            return allusers;
        }

        public IList<ITraining> GetAllTrainings()
        {
            var copy = new List<ITraining>(trainings);
            foreach (var item in copy)
            {
                item.RegisteredNumber = usersInTrainings.Count(r => r.TrainingID == item.ID);
            }
            return copy;
        }

        public void SetAdminRole(string email, bool isAdmin)
        {
            if( users.ContainsKey(email))
            {
                users[email].IsAdmin = isAdmin;
            }
        }

        public void SetUserCategory(string email, UserCategories category)
        {
            if (users.ContainsKey(email))
            {
                users[email].Category = category;
            }
        }

        public int UpdateCountOfFreeSignUps(string email, int changeNumberOfSignUps)
        {
            if (users.ContainsKey(email))
            {
                users[email].NumberOfFreeSignUps += changeNumberOfSignUps;
                if (users[email].NumberOfFreeSignUps < 0)
                {
                    users[email].NumberOfFreeSignUps = 0;
                }
                return users[email].NumberOfFreeSignUps;
            }
            return 0;
        }

        public void SetTrainer(string trainingID, string trainer)
        {
            var training = trainings.FirstOrDefault(r => r.ID == trainingID);
            if(training != null)
            {
                training.Trainer = trainer;
            }
        }

        public IList<IUser> GetUsersInTraining(string trainingID)
        {
            var usersInTraining = usersInTrainings.Where(r => r.TrainingID == trainingID);

            return usersInTraining.Select(r => GetUserData(r.Email)).ToList();
        }

        private class UserInTraining
        {
          public  string Email { get; set; }
            public DateTime Time { get;  set; }
            public string TrainingID { get; set; }
        }

        private class RegisteredUser : RegisterModel
        {
            public UserCategories Category { get; internal set; }
            public bool IsAdmin { get; set; }
            public int NumberOfFreeSignUps { get; internal set; }
        }
    }

}