using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Common.Data;
using Speranza.Database.Data.Interfaces;
using System.Data.SqlClient;
using Speranza.Database.Data;

namespace Speranza.Database
{
    public class Database : IDatabaseGateway
    {
        private const string SERVER_NAME = "localhost\\NTB";
        private const string DATABASE_NAME = "SPERANZADB";
        private SqlConnection connection;
        private const string LAST_TEMPLATE_GENERATION_DATE = "LastTemplateGenerationDate";
        private const string SETTINGS_SIGN_OFF_LIMIT = "SignOffLimit";

        public Database()
        {
            string connetionString = null;
            connetionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zuzka\Source\Repos\Speranza\SperanzaDB.mdf;Integrated Security=True;Connect Timeout=30";

            //connetionString = "Data Source=" + SERVER_NAME + ";Initial Catalog=" + DATABASE_NAME;
            connection = new SqlConnection(connetionString);
            connection.Open();
            connection.Close();
        }

        //usersInTrainings.Add(new UserInTraining() { Email = email, TrainingID = trainingID, Time = time });
        public void AddUserToTraining(string email, string trainingID, DateTime timeOfSignUp)
        {
            string sql = string.Format("INSERT INTO UsersInTrainings(email,trainingID,time) VALUES( '{0}','{1}','{2}');",
              email, trainingID, GetDateFormat(timeOfSignUp));

            ExecuteSql(sql);
        }

        public void AllowSigningUpToTrainings(string email)
        {
            throw new NotImplementedException();
        }

        public void CancelTraining(string trainingID)
        {
            throw new NotImplementedException();
        }

        public void ChangePassword(string email, string newpasswordhash)
        {
            throw new NotImplementedException();
        }

        public void ConfirmParticipation(string trainingID, string email)
        {
            throw new NotImplementedException();
        }

        public void CreateNewTraining(string trainingID, DateTime dateTime, string trainer, string trainingDescription, int capacity)
        {
            string sql = string.Format("INSERT INTO Trainings(Id,time,trainer,description,capacity) VALUES( '{0}','{1}','{2}','{3}',{4});",
               trainingID, GetDateFormat(dateTime), trainer, trainingDescription, capacity);

            ExecuteSql(sql);
        }

        public void CreateRecurringTrainingTemplate(RecurringTrainingTemplate recurringTrainingTemplate)
        {
            throw new NotImplementedException();
        }

        public void DisproveParticipation(string trainingID, string email)
        {
            throw new NotImplementedException();
        }


        public void ForbidSigningUpToTrainings(string email)
        {
            throw new NotImplementedException();
        }
        //return users.Where(r => r.Value.IsAdmin).Select(r => GetUserData(r.Value.Email)).ToList();
        public IList<IUser> GetAdmins()
        {
            string sql = string.Format("SELECT email,category,isAdmin,name,surname,phoneNumber,numberOfFreeSignUps FROM Users WHERE isAdmin=1;");
            var objects = ExecuteSqlWithResult(sql);
            var users = new List<IUser>();
            foreach (var item in objects)
            {
                User user = new User();
                user.Email = (string)item[0];
                user.Category = (UserCategories)item[1];
                user.IsAdmin = (byte)item[2] == 1;
                user.Name = (string)item[3];
                user.Surname = (string)item[4];
                user.PhoneNumber = (string)item[5];
                user.NumberOfFreeSignUpsOnSeasonTicket = (int)item[6];
                string sql2 = string.Format("Select Count(*) from UsersInTrainings Where email ='{0}' AND (SELECT time from Trainings WHERE Id= trainingID) < GetDate();", user.Email);
                var objects2 = ExecuteSqlWithResult(sql2);
                user.NumberOfPastTrainings = (int)objects2[0][0];
                users.Add(user);
            }

            return users;
        }

        public bool GetAllowedToSignUpFlag(string email)
        {
            string sql = string.Format("SELECT isSignUpAllowed FROM Users WHERE email ='{0}';", email);
            var objects = ExecuteSqlWithResult(sql);
            return (byte)objects[0][0] == 1;
        }

        public IList<ITraining> GetAllTrainings()
        {
            string sql = string.Format("SELECT * FROM Trainings ;");
            var objects = ExecuteSqlWithResult(sql);
            IList<ITraining> trainings = new List<ITraining>();
            foreach (var item in objects)
            {
                Training training = new Training();
                training.ID = (string)item[0];
                training.Capacity = (int)item[1];
                training.Description = (string)item[2];
                training.Time = (DateTime)item[3];
                training.Trainer = (string)item[4];
                string sql2 = string.Format("SELECT Count(*) FROM UsersInTrainings WHERE trainingID ='{0}';", training.ID);
                var objects2 = ExecuteSqlWithResult(sql2);
                training.RegisteredNumber = (int)objects2[0][0];

                trainings.Add(training);
            }

            return trainings;
        }

        public IList<IUser> GetAllUsers()
        {
            string sql = string.Format("SELECT email,category,isAdmin,name,surname,phoneNumber,numberOfFreeSignUps FROM Users ;");
            var objects = ExecuteSqlWithResult(sql);
            var users = new List<IUser>();
            foreach (var item in objects)
            {
                User user = new User();
                user.Email = (string)item[0];
                user.Category = (UserCategories)item[1];
                user.IsAdmin = (byte)item[2] == 1;
                user.Name = (string)item[3];
                user.Surname = (string)item[4];
                user.PhoneNumber = (string)item[5];
                user.NumberOfFreeSignUpsOnSeasonTicket = (int)item[6];
                string sql2 = string.Format("Select Count(*) from UsersInTrainings Where email ='{0}' AND (SELECT time from Trainings WHERE Id= trainingID) < GetDate();", user.Email);
                var objects2 = ExecuteSqlWithResult(sql2);
                user.NumberOfPastTrainings = (int)objects2[0][0];
                users.Add(user);
            }

            return users;
        }


        //return usersInTrainings.Where(r => r.ZeroEntranceFlag == true).ToList();
        public IList<IUserInTraining> GetAllUsersInTrainingWithZeroEntranceFlag()
        {
            string sql = string.Format("SELECT * FROM UsersInTrainings WHERE zeroEntranceFlag =1;");
            var objects = ExecuteSqlWithResult(sql);
            var usersInTrainings = new List<IUserInTraining>();
            foreach (var item in objects)
            {
                var userInTraining = new UserInTraining();
                userInTraining.Email = (string)item[0];
                userInTraining.TrainingID = (string)item[1];
                userInTraining.Time = (DateTime)item[2];
                userInTraining.ParticipationConfirmed = (byte)item[3] == 1;
                userInTraining.ParticipationDisproved = (byte)item[4] == 1;
                userInTraining.AlreadyProcessed = (byte)item[5] == 1;
                userInTraining.ZeroEntranceFlag = (byte)item[6] == 1;
                usersInTrainings.Add(userInTraining);
            }

            return usersInTrainings;
        }

        public IList<ITraining> GetDayTrainings(DateTime date)
        {

            string sql = string.Format("SELECT * FROM Trainings WHERE CONVERT(date, time) ='{0}';", date.ToString("yyyy-MM-dd"));
            var objects = ExecuteSqlWithResult(sql);
            IList<ITraining> trainings = new List<ITraining>();
            foreach (var item in objects)
            {
                Training training = new Training();
                training.ID = (string)item[0];
                training.Capacity = (int)item[1];
                training.Description = (string)item[2];
                training.Time = (DateTime)item[3];
                training.Trainer = (string)item[4];
                string sql2 = string.Format("SELECT Count(*) FROM UsersInTrainings WHERE trainingID ='{0}';", training.ID);
                var objects2 = ExecuteSqlWithResult(sql2);
                training.RegisteredNumber = (int)objects2[0][0];

                trainings.Add(training);
            }

            return trainings;


        }

        //return usersInTrainings.Where(r => r.TrainingID == trainingID).Select(r => r.Email).ToList();
        public IList<string> GetEmailsOfAllUsersInTraining(string trainingID)
        {
            string sql = string.Format("SELECT email FROM UsersInTrainings WHERE trainingID='{0}';", trainingID);
            var objects = ExecuteSqlWithResult(sql);
            IList<string> emails = new List<string>();
            foreach (var item in objects)
            {
                string email = (string)item[0];

                emails.Add(email);
            }

            return emails;
        }



        //return (DateTime) settings[LAST_TEMPLATE_GENERATION_DATE];
        public DateTime GetLastTemplateGenerationDate()
        {
            string sql = string.Format("SELECT value FROM Settings WHERE Id ='{0}';", LAST_TEMPLATE_GENERATION_DATE);
            var objects = ExecuteSqlWithResult(sql);
            DateTime date = DateTime.Parse((string)objects[0][0]);
            return date;
        }

        public IList<IUserInTraining> GetNonProcessedUsersInTrainingBeforeDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public int GetNumberOfVisits(string email, DateTime currentDate)
        {

            string sql = string.Format("SELECT COUNT(*) FROM UsersInTrainings WHERE email ='{0}' AND time <'{1}' AND participationDisproved = 0;", email, GetDateFormat(currentDate));
            var objects = ExecuteSqlWithResult(sql);

            if (objects.Count == 1)
            {
                return (int)objects[0][0];
            }
            return 0;
        }

        private string GetDateFormat(DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss");
        }

        public int GetSignOffLimit()
        {
            string sql = string.Format("SELECT value FROM Settings WHERE Id ='{0}';", SETTINGS_SIGN_OFF_LIMIT);
            var objects = ExecuteSqlWithResult(sql);
            int limit = int.Parse((string)objects[0][0]);
            return limit;
        }

        public IList<IRecurringTrainingTemplate> GetTemplates()
        {
            string sql = string.Format("SELECT * FROM RecurringTemplate;");
            var objects = ExecuteSqlWithResult(sql);
            IList<IRecurringTrainingTemplate> templates = new List<IRecurringTrainingTemplate>();
            foreach (var item in objects)
            {
                var template = new RecurringTrainingTemplate();
                template.Capacity = (int)item[1];
                template.Day = (int)item[2];
                template.Time = (int)item[3];
                template.Description = (string)item[4];
                template.Trainer = (string)item[5];
                template.ValidFrom = (DateTime)item[6];

                templates.Add(template);
            }

            return templates;
        }

        //return templates.Where(r => r.Day == day).ToList();
        public IList<IRecurringTrainingTemplate> GetTemplatesForTheDay(int day)
        {
            string sql = string.Format("SELECT * FROM RecurringTemplate WHERE day='{0}';", day);
            var objects = ExecuteSqlWithResult(sql);
            IList<IRecurringTrainingTemplate> templates = new List<IRecurringTrainingTemplate>();
            foreach (var item in objects)
            {
                var template = new RecurringTrainingTemplate();
                template.Capacity = (int)item[1];
                template.Day = (int)item[2];
                template.Time = (int)item[3];
                template.Description = (string)item[4];
                template.Trainer = (string)item[5];
                template.ValidFrom = (DateTime)item[6];

                templates.Add(template);
            }

            return templates;
        }

        //var t = trainings.FirstOrDefault(r => r.ID == trainingID);
        //    if (t != null)
        //    {
        //        t.RegisteredNumber = usersInTrainings.Count(r => r.TrainingID == t.ID);
        //    }
        //    return t;
        public ITraining GetTrainingData(string trainingID)
        {
            string sql = string.Format("SELECT * FROM Trainings WHERE Id ='{0}';", trainingID);
            var objects = ExecuteSqlWithResult(sql);
            if (objects.Count == 0)
            {
                return null;
            }
            Training training = new Training();
            training.ID = (string)objects[0][0];
            training.Capacity = (int)objects[0][1];
            training.Description = (string)objects[0][2];
            training.Time = (DateTime)objects[0][3];
            training.Trainer = (string)objects[0][4];
            string sql2 = string.Format("SELECT Count(*) FROM UsersInTrainings WHERE trainingID ='{0}';", training.ID);
            var objects2 = ExecuteSqlWithResult(sql2);
            training.RegisteredNumber = (int)objects2[0][0];

            return training;
        }

        //return trainings.Count(r => r.Time > date);
        public int GetTrainingsCountAfterDate(DateTime date)
        {
            string sql = string.Format("SELECT Count(*) FROM Trainings WHERE time >'{0}';", GetDateFormat(date));
            var objects = ExecuteSqlWithResult(sql);
            return (int)objects[0][0];
        }

        public int GetTrainingsCountBeforeDate(DateTime date)
        {
            string sql = string.Format("SELECT Count(*) FROM Trainings WHERE time<'{0}';", GetDateFormat(date));
            var objects = ExecuteSqlWithResult(sql);
            return (int)objects[0][0];
        }


        public IList<ITraining> GetTrainingsForUser(string email)
        {
            string sql = string.Format("Select T.* from Trainings T, UsersInTrainings U Where U.email ='{0}' AND U.trainingID = T.Id;", email);
            var objects = ExecuteSqlWithResult(sql);
            var trainings = new List<ITraining>();

            foreach (var item in objects)
            {
                var training = new Training();
                training.ID = (string)item[0];
                training.Capacity = (int)item[1];
                training.Description = (string)item[2];
                training.Time = (DateTime)item[3];
                training.Trainer = (string)item[4];
                string sql2 = string.Format("SELECT Count(*) FROM UsersInTrainings WHERE trainingID ='{0}';", training.ID);
                var objects2 = ExecuteSqlWithResult(sql2);
                training.RegisteredNumber = (int)objects2[0][0];

                trainings.Add(training);
            }

            return trainings;

        }


        //        user.NumberOfPastTrainings = usersInTrainings.Count(r => r.Email == email && trainings.First(p => p.ID == r.TrainingID).Time<DateTime.Now);
        //        return user;
        //    }
        public IUser GetUserData(string email)
        {
            string sql = string.Format("SELECT email,category,isAdmin,name,surname,phoneNumber,numberOfFreeSignUps FROM Users WHERE email ='{0}';", email);
            var objects = ExecuteSqlWithResult(sql);

            if (objects.Count == 1)
            {
                User user = new User();
                user.Email = (string)objects[0][0];
                user.Category = (UserCategories)objects[0][1];
                user.IsAdmin = (byte)objects[0][2] == 1;
                user.Name = (string)objects[0][3];
                user.Surname = (string)objects[0][4];
                user.PhoneNumber = (string)objects[0][5];
                user.NumberOfFreeSignUpsOnSeasonTicket = (int)objects[0][6];
                string sql2 = string.Format("Select Count(*) from UsersInTrainings Where email ='{0}' AND (SELECT time from Trainings WHERE Id= trainingID) < GetDate();", email);
                var objects2 = ExecuteSqlWithResult(sql2);
                user.NumberOfPastTrainings = (int)objects2[0][0];
                return user;
            }

            return null;
        }

        public IList<IUser> GetUsersInTraining(string trainingID)
        {
            throw new NotImplementedException();
        }


        //return usersInTrainings.Any(r => r.Email == email && r.TrainingID == trainingID);
        public bool IsUserAlreadySignedUpInTraining(string email, string trainingID)
        {
            string sql = string.Format("SELECT COUNT(*) FROM UsersInTrainings WHERE email ='{0}' AND trainingID ='{1}';", email, trainingID);
            var objects = ExecuteSqlWithResult(sql);

            return (int)objects[0][0] == 1;
        }


        public IUser LoadUser(string email)
        {
            string sql = string.Format("SELECT email,category,isAdmin,password FROM Users WHERE email ='{0}';", email);
            var objects = ExecuteSqlWithResult(sql);

            if (objects.Count == 1)
            {
                User user = new User();
                user.Email = (string)objects[0][0];
                user.Category = (UserCategories)objects[0][1];
                user.IsAdmin = (byte)objects[0][2] == 1;
                user.PasswordHash = (string)objects[0][3];

                return user;
            }

            return null;
        }

        public void RegisterNewUser(string email, string name, string password, string phoneNumber, string surname)
        {
            //INSERT INTO table_name(column1, column2, column3, ...) VALUES(value1, value2, value3, ...);

            string sql = string.Format("INSERT INTO Users(email,name,surname,phoneNumber,password) VALUES( '{0}','{1}','{2}','{3}','{4}');",
                email, name, surname, phoneNumber, password);

            ExecuteSql(sql);
        }

        //var template = templates.Find(r => r.Day == day && r.Time == time);
        //templates.Remove(template);
        public void RemoveTrainingTemplate(int day, int time)
        {
            string sql = string.Format("DELETE FROM RecurringTemplate WHERE day ={0} AND time ={1};", day, time);

            ExecuteSql(sql);
        }

        public void RemoveUserFromTraining(string email, string trainingID)
        {
            string sql = string.Format("DELETE FROM UsersInTrainings WHERE email ='{0}' AND trainingID ='{1}';",
              email, trainingID);

            ExecuteSql(sql);
        }

        //if (users.ContainsKey(email))
        //  {
        //      users[email].IsAdmin = isAdmin;
        //  }
        public void SetAdminRole(string email, bool isAdmin)
        {
            string sql = string.Format("Update Users SET isAdmin={0} WHERE email ='{1}';", isAdmin ? 1 : 0, email);
            ExecuteSql(sql);
        }

        //userInTraining.AlreadyProcessed = true;
        public void SetAlreadyProcessedFlag(IUserInTraining userInTraining)
        {
            string sql = string.Format("Update UsersInTrainings SET alreadyProcessed=1 WHERE email ='{0}' AND trainingID='{1}';", userInTraining.Email, userInTraining.TrainingID);
            ExecuteSql(sql);
        }

        public void SetLastTemplateGenerationDate(DateTime dateTime)
        {
            string sql = string.Format("Update Settings SET value='{0}' WHERE Id ='{1}';", GetDateFormat(dateTime), LAST_TEMPLATE_GENERATION_DATE);
            ExecuteSql(sql);
        }
        //settings[SETTINGS_SIGN_OFF_LIMIT] = hoursLimit;
        public void SetSignOffLimit(int hoursLimit)
        {
            string sql = string.Format("Update Settings SET value={0} WHERE Id ='{1}';", hoursLimit, SETTINGS_SIGN_OFF_LIMIT);
            ExecuteSql(sql);
        }

        public void SetTrainer(string trainingID, string trainer)
        {
            string sql = string.Format("UPDATE Trainings SET trainer = '{0}'  WHERE Id ='{1}';", trainer, trainingID);
            ExecuteSql(sql);
        }

        public void SetTrainingCapacity(string trainingID, int capacity)
        {
            string sql = string.Format("UPDATE Trainings SET capacity = {0}  WHERE Id ='{1}';", capacity, trainingID);
            ExecuteSql(sql);
        }

        public void SetTrainingDescription(string trainingID, string trainingDescription)
        {
            string sql = string.Format("UPDATE Trainings SET description = '{0}'  WHERE Id ='{1}';", trainingDescription, trainingID);
            ExecuteSql(sql);
        }

        public void SetUserCategory(string email, UserCategories category)
        {
            throw new NotImplementedException();
        }

        public void SetZeroEntranceFlag(IUserInTraining userInTraining, bool flag)
        {
            throw new NotImplementedException();
        }

        public void SignOutUserFromAllTrainingsAfterDate(string email, DateTime date)
        {
            throw new NotImplementedException();
        }

        public int UpdateCountOfFreeSignUps(string email, int changeNumberOfSignUps)
        {
            throw new NotImplementedException();
        }


        //if (users.ContainsKey(email))
        //{
        //    users[email].Name = name;
        //    users[email].Surname = surname;
        //    users[email].PhoneNumber = phoneNumber;
        //}
        public void UpdateUserData(string email, string name, string surname, string phoneNumber)
        {
            string sql = string.Format("UPDATE Users SET name = '{0}',surname= '{1}',phoneNumber='{2}'  WHERE email ='{3}';", name, surname, phoneNumber, email);
            ExecuteSql(sql);
        }

        //SELECT column1, column2, ...FROM table_name;
        public bool UserExists(string email)
        {
            string sql = string.Format("SELECT COUNT(*) FROM Users WHERE email ='{0}';", email);
            var objects = ExecuteSqlWithResult(sql);
            if ((int)objects[0][0] == 1)
                return true;
            return false;
        }

        private void ExecuteSql(string sql)
        {
            try
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
            catch
            {

            }
            finally
            {
                connection.Close();
            }
        }

        private IList<object[]> ExecuteSqlWithResult(string sql)
        {
            IList<object[]> rows = new List<object[]>();
            try
            {
                connection.Open();
                var command = new SqlCommand(sql, connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    object[] columns = new object[reader.FieldCount];
                    reader.GetValues(columns);
                    rows.Add(columns);
                }
            }
            catch
            {

            }
            finally
            {
                connection.Close();
            }
            return rows;
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
    }
}
