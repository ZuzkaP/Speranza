using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Models;
using Speranza.Database.Data.Interfaces;
using Speranza.Database.Data;
using Speranza.Common.Data;
using Speranza.Models.Interfaces;
using Speranza.Smtp.Interfaces;

namespace Speranza.Database
{
    public class InMemoryDatabase : IDatabaseGateway
    {
        private const string SETTINGS_SIGN_OFF_LIMIT = "SignOffLimit";
        Dictionary<string, RegisteredUser> users;
        Dictionary<string, object> settings;
        Dictionary<string, string> tokens;
        List<ITraining> trainings;
        List<IRecurringTrainingTemplate> templates;
        List<IUserInTraining> usersInTrainings;
        List<IUserNotificationMessage> messages;
        static InMemoryDatabase database;
        private const string LAST_TEMPLATE_GENERATION_DATE = "LastTemplateGenerationDate";

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
            usersInTrainings = new List<IUserInTraining>();
            messages = new List<IUserNotificationMessage>();
            settings = new Dictionary<string, object>();
            tokens = new Dictionary<string, string>();
            templates = new List<IRecurringTrainingTemplate>();
            users.Add("admin", new RegisteredUser() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "Admin", Surname = "Admin", PhoneNumber = "1234", IsAdmin = true, Category = UserCategories.Silver });

            users.Add("miro", new RegisteredUser() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "Miro", Surname = "Pavlicko", PhoneNumber = "1234", IsAdmin = false, NumberOfFreeSignUps = 10 });
            users.Add("Zuzka", new RegisteredUser() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "Zuzana", Surname = "papalova", PhoneNumber = "1234", IsAdmin = false, Category = UserCategories.Gold, NumberOfFreeSignUps = 7 });
            users.Add("Jozko", new RegisteredUser() { /*"pass1 (hashed)"*/Password = "/4SrsZcLUnq/LpZTmllEyETvXELfPGR5zafWRUPN8+EyaHjziFh8OqiRO2rtZfQI+hdyNjV2B8It910eHvONIg==", Name = "jozko", Surname = "Mrkvicka", PhoneNumber = "1234", IsAdmin = false, Category = UserCategories.Gold, NumberOfFreeSignUps = 8 });

            trainings = new List<ITraining>();
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            trainings.Add(PrepareTraining(new DateTime(year, month, day, 12, 00, 00).AddDays(2), "training c.1", "Zuzka", 10));
            trainings.Add(PrepareTraining(new DateTime(year, month, day, 14, 00, 00).AddDays(2), "training c.2", "Dano", 10));
            trainings.Add(PrepareTraining(new DateTime(year, month, day, 08, 00, 00).AddDays(12), "training c.4", "Filip", 10));
            trainings.Add(PrepareTraining(new DateTime(year, month, day, 14, 00, 00).AddDays(8), "training c.5", "Filip", 10));
            trainings.Add(PrepareTraining(new DateTime(year, month, day, 09, 00, 00).AddDays(4), "training c.3", "Filip", 10));
            trainings.Add(PrepareTraining(new DateTime(year, month, day, 09, 00, 00).AddDays(-10), "training c.3", "Filip", 10));
            usersInTrainings.Add(new UserInTraining() { Email = "admin", TrainingID = trainings[0].ID });
            usersInTrainings.Add(new UserInTraining() { Email = "admin", TrainingID = trainings[5].ID });

            settings.Add(SETTINGS_SIGN_OFF_LIMIT, 4);
            settings.Add(LAST_TEMPLATE_GENERATION_DATE, DateTime.MinValue);
        }

        private ITraining PrepareTraining(DateTime dateTime, string v1, string v2, int v3)
        {

            return new Training(Guid.NewGuid().ToString(), dateTime, v1, v2, v3,false);
        }

        public void RegisterNewUser(string email, string name, string password, string phoneNumber, string surname)
        {
            RegisteredUser user = new RegisteredUser();
            user.Name = name;
            user.Email = email;
            user.IsAdmin = false;
            user.Password = password;
            user.PhoneNumber = phoneNumber;
            user.Surname = surname;

            users.Add(email, user);
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


        public IUser LoadUser(string series,string token)
        {
          if(tokens.ContainsKey(series) && tokens[series] == token)
            {
                var emails = users.Where(r=>r.Value.Series == series).Select(r=>r.Key).ToList();
                if(emails.Count == 1)
                { 
                    User user = new User();
                    user.Email = emails[0];
                    user.Category = users[emails[0]].Category;
                    user.IsAdmin = users[emails[0]].IsAdmin;
                    return user;
                }
               
            }
            return null;
           
        }
        public IList<ITraining> GetDayTrainings(DateTime date)
        {
            var t = trainings.Where(r => r.Time.Date == date.Date).ToList();
            foreach (var item in t)
            {
                item.RegisteredNumber = usersInTrainings.Count(r => r.TrainingID == item.ID);
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
                user.NumberOfPastTrainings = usersInTrainings.Count(r => r.Email == email && trainings.First(p => p.ID == r.TrainingID).Time < DateTime.Now);
                return user;
            }

            return null;
        }

        public void UpdateUserData(string email, string name, string surname, string phoneNumber)
        {
            if (users.ContainsKey(email))
            {
                users[email].Name = name;
                users[email].Surname = surname;
                users[email].PhoneNumber = phoneNumber;
            }
        }

        public ITraining GetTrainingData(string trainingID)
        {
            var t = trainings.FirstOrDefault(r => r.ID == trainingID);
            if (t != null)
            {
                t.RegisteredNumber = usersInTrainings.Count(r => r.TrainingID == t.ID);
            }
            return t;
        }

        public void AddUserToTraining(string email, string trainingID, DateTime time)
        {
            usersInTrainings.Add(new UserInTraining() { Email = email, TrainingID = trainingID, Time = time });
        }

        public bool IsUserAlreadySignedUpInTraining(string email, string trainingID)
        {
            return usersInTrainings.Any(r => r.Email == email && r.TrainingID == trainingID);
        }

        public void RemoveUserFromTraining(string email, string id)
        {
            IUserInTraining toBeRemoved = usersInTrainings.FirstOrDefault(r => r.Email == email && r.TrainingID == id);
            if (toBeRemoved != null)
            {
                usersInTrainings.Remove(toBeRemoved);
            }
        }

        public IList<ITraining> GetTrainingsForUser(string email)
        {
            List<IUserInTraining> trainingsForUser = usersInTrainings.Where(r => r.Email == email).ToList();
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
            if (users.ContainsKey(email))
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
            if (training != null)
            {
                training.Trainer = trainer;
            }
        }

        public IList<IUser> GetUsersInTraining(string trainingID)
        {
            var usersInTraining = usersInTrainings.Where(r => r.TrainingID == trainingID);

            var users = usersInTraining.Select(r => {
                var user = GetUserData(r.Email);
                user.SignUpTime = r.Time;
                return user;
            }).ToList();

            foreach (var item in users)
            {
                if (usersInTraining.First(r => r.Email == item.Email).ParticipationConfirmed || usersInTraining.First(r => r.Email == item.Email).ParticipationDisproved)
                {
                    item.ParticipationSet = true;
                }
                if (usersInTraining.First(r => r.Email == item.Email).ParticipationDisproved)
                {
                    item.ParticipationDisapproved = true;
                }
            }
            return users;
        }

        public void SetTrainingDescription(string trainingID, string trainingDescription)
        {
            var training = trainings.FirstOrDefault(r => r.ID == trainingID);
            if (training != null)
            {
                training.Description = trainingDescription;
            }
        }

        public void SetTrainingCapacity(string trainingID, int capacity)
        {
            var training = trainings.FirstOrDefault(r => r.ID == trainingID);
            if (training != null)
            {
                training.Capacity = capacity;
            }
        }

     
        public void CancelTraining(string trainingID)
        {
            var training = trainings.FirstOrDefault(r => r.ID == trainingID);
            if (training != null)
            {
                trainings.Remove(training);
            }

        }

        public void SetSignOffLimit(int hoursLimit)
        {
            settings[SETTINGS_SIGN_OFF_LIMIT] = hoursLimit;
        }

        public int GetSignOffLimit()
        {
            return (int)settings[SETTINGS_SIGN_OFF_LIMIT];
        }

        public void ChangePassword(string email, string newpasswordhash)
        {
            if (users.ContainsKey(email))
            {
                users[email].Password = newpasswordhash;
            }
        }

        public void CreateRecurringTrainingTemplate(RecurringTrainingTemplate recurringTrainingTemplate)
        {
            if (templates.Find(r => r.Day == recurringTrainingTemplate.Day && r.Time == recurringTrainingTemplate.Time) == null)
            {
                templates.Add(recurringTrainingTemplate);
            }
        }

        public IList<IRecurringTrainingTemplate> GetTemplates()
        {
            return templates;
        }

        public void RemoveTrainingTemplate(int day, int time)
        {
            var template = templates.Find(r => r.Day == day && r.Time == time);
            templates.Remove(template);
        }

        public IList<IRecurringTrainingTemplate> GetTemplatesForTheDay(int day)
        {
            return templates.Where(r => r.Day == day).ToList();
        }

        public void SetLastTemplateGenerationDate(DateTime dateTime)
        {
            settings[LAST_TEMPLATE_GENERATION_DATE] = dateTime;
        }

        public DateTime GetLastTemplateGenerationDate()
        {
            return (DateTime)settings[LAST_TEMPLATE_GENERATION_DATE];
        }

        public int GetTrainingsCountAfterDate(DateTime date)
        {
            return trainings.Count(r => r.Time > date);
        }

        public int GetTrainingsCountBeforeDate(DateTime date)
        {
            return trainings.Count(r => r.Time < date);
        }

        public void ConfirmParticipation(string trainingID, string email)
        {
            usersInTrainings.First(r => r.Email == email && r.TrainingID == trainingID).ParticipationConfirmed = true;
        }

        public void DisproveParticipation(string trainingID, string email)
        {
            usersInTrainings.First(r => r.Email == email && r.TrainingID == trainingID).ParticipationDisproved = true;
        }

        public void SignOutUserFromAllTrainingsAfterDate(string email, DateTime date)
        {
            var userTrainings = usersInTrainings.Where(r => r.Email == email && trainings.First(s => s.ID == r.TrainingID).Time > date).ToList();
            foreach (var item in userTrainings)
            {
                usersInTrainings.Remove(item);
            }
        }

        public void ForbidSigningUpToTrainings(string email)
        {
            users[email].SignUpAllowed = false;
        }

        public void AllowSigningUpToTrainings(string email)
        {
            users[email].SignUpAllowed = true;
        }

        public bool GetAllowedToSignUpFlag(string email)
        {
            return users[email].SignUpAllowed;
        }

        public int GetNumberOfVisits(string email, DateTime currentDate)
        {
            return usersInTrainings.Count(r => r.Email == email && r.Time < currentDate && !r.ParticipationDisproved);
        }

        public IList<IUserInTraining> GetNonProcessedUsersInTrainingBeforeDate(DateTime date)
        {
            return usersInTrainings.Where(r => r.AlreadyProcessed == false && GetTrainingData(r.TrainingID).Time <= date).ToList();
        }

        public void SetAlreadyProcessedFlag(IUserInTraining userInTraining)
        {
            userInTraining.AlreadyProcessed = true;
        }

        public IList<string> GetEmailsOfAllUsersInTraining(string trainingID)
        {
           return usersInTrainings.Where(r => r.TrainingID == trainingID).Select(r => r.Email).ToList();
        }

        public void SetZeroEntranceFlag(IUserInTraining userInTraining, bool flag)
        {
            userInTraining.ZeroEntranceFlag = flag;
        }

        public IList<IUserInTraining> GetAllUsersInTrainingWithZeroEntranceFlag()
        {
            return usersInTrainings.Where(r => r.ZeroEntranceFlag == true).ToList();
        }

        public IList<IUser> GetAdmins()
        {
            return users.Where(r => r.Value.IsAdmin).Select(r => GetUserData(r.Value.Email)).ToList();
        }

        public void CreateNewTraining(string trainingID, DateTime dateTime, string trainer, string trainingDescription, int capacity)
        {
            CreateNewTraining(trainingID, dateTime, trainer, trainingDescription, capacity, false);
        }

        public void CreateNewTraining(string trainingID, DateTime dateTime, string trainer, string trainingDescription, int capacity, bool isFromTemplate)
        {
            trainings.Add(new Training(trainingID, dateTime, trainingDescription, trainer, capacity, isFromTemplate));
        }

        public void SetRememberMe(string email, string series, string token)
        {
            tokens.Add(series, token);
            users[email].Series = series;
        }

        public void CancelRememberMe(string email)
        {
            if (users[email].Series != null && tokens.ContainsKey(users[email].Series))
            {
                tokens.Remove(users[email].Series);
            }
            users[email].Series = null;
        }

        public void CleanUpTokens()
        {
            var forgotten = new List<string>();
            foreach (var token in tokens.Keys)
            {
               if(users.Count(r=>r.Value.Series == token) == 0)
                {
                    forgotten.Add(token);
                }
            }

            foreach (var item in forgotten)
            {
                tokens.Remove(item);
            }
        }

        public void WriteToLog(string eMessage, Email email)
        {
            string message = string.Format(" '{0}'---'{1}'---'{2}'---'{3}'---'{4}'\n",DateTime.Now.ToString("O"),email.Receiver,email.Subject,email.Body,eMessage);
            
            using (StreamWriter outfile = new StreamWriter("C:\\Users\\zuzana.papalova\\Documents\\log.txt"))
            {
                outfile.Write(message);
            }
        }

        public void AddNewMessage(DateTime @from, DateTime to, string message)
        {
            messages.Add(new UserNotificationMessage(from,to,message));
        }

        public string GetMessageForCurrentDate()
        {
            throw new NotImplementedException();
        }

        private class UserInTraining : IUserInTraining
        {
            public string Email { get; set; }
            public DateTime Time { get; set; }
            public string TrainingID { get; set; }
            public bool ParticipationConfirmed { get; set; }
            public bool ParticipationDisproved { get; set; }
            public bool AlreadyProcessed { get; set; }
            public bool ZeroEntranceFlag { get; set; }
        }

        private class RegisteredUser : RegisterModel
        {
            public UserCategories Category { get; internal set; }
            public bool IsAdmin { get; set; }
            public int NumberOfFreeSignUps { get; internal set; }
            public bool SignUpAllowed { get; internal set; }
            public string Series { get; internal set; }

            public RegisteredUser()
            {
                SignUpAllowed = true;
            }
        }
    }

}