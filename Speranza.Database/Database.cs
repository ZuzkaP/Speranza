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

        public Database()
        {
            string connetionString = null;
            connetionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Zuzka\Source\Repos\Speranza\SperanzaDB.mdf;Integrated Security=True;Connect Timeout=30";

            //connetionString = "Data Source=" + SERVER_NAME + ";Initial Catalog=" + DATABASE_NAME;
            connection = new SqlConnection(connetionString);
            connection.Open();
            connection.Close();
        }
        public void AddUserToTraining(string email, string trainingID, DateTime timeOfSignUp)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        public IList<IUser> GetAdmins()
        {
            throw new NotImplementedException();
        }

        public bool GetAllowedToSignUpFlag(string email)
        {
            string sql = string.Format("SELECT isSignUpAllowed FROM Users WHERE email ='{0}';", email);
            var objects = ExecuteSqlWithResult(sql);
            return (byte)objects[0][0] == 1;
        }

        public IList<ITraining> GetAllTrainings()
        {
            throw new NotImplementedException();
        }

        public IList<IUser> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public IList<IUserInTraining> GetAllUsersInTrainingWithZeroEntranceFlag()
        {
            throw new NotImplementedException();
        }

        public IList<ITraining> GetDayTrainings(DateTime date)
        {

            string sql = string.Format("SELECT * FROM Trainings WHERE CONVERT(date, T.time) ='{0}';", date.ToString("yyyy-MM-dd"));
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

        public IList<string> GetEmailsOfAllUsersInTraining(string trainingID)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IList<IRecurringTrainingTemplate> GetTemplates()
        {
            throw new NotImplementedException();
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

        public ITraining GetTrainingData(string trainingID)
        {
            throw new NotImplementedException();
        }

        public int GetTrainingsCountAfterDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public int GetTrainingsCountBeforeDate(DateTime date)
        {
            throw new NotImplementedException();
        }


        public IList<ITraining> GetTrainingsForUser(string email)
        {
            string sql = string.Format("Select T.* from Trainings T, UsersInTrainings U Where U.email ='{0}' AND U.trainingID = T.Id;",email);
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

        public bool IsUserAlreadySignedUpInTraining(string email, string trainingID)
        {
            throw new NotImplementedException();
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


        public void RemoveTrainingTemplate(int day, int time)
        {
            throw new NotImplementedException();
        }

        public void RemoveUserFromTraining(string email, string trainingID)
        {
            throw new NotImplementedException();
        }

        public void SetAdminRole(string email, bool isAdmin)
        {
            throw new NotImplementedException();
        }

        public void SetAlreadyProcessedFlag(IUserInTraining userInTraining)
        {
            throw new NotImplementedException();
        }

        public void SetLastTemplateGenerationDate(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public void SetSignOffLimit(int hoursLimit)
        {
            throw new NotImplementedException();
        }

        public void SetTrainer(string trainingID, string trainer)
        {
            throw new NotImplementedException();
        }

        public void SetTrainingCapacity(string trainingID, int capacity)
        {
            throw new NotImplementedException();
        }

        public void SetTrainingDescription(string trainingID, string trainingDescription)
        {
            throw new NotImplementedException();
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
            string sql = string.Format("UPDATE Users SET name = '{0}',surname= '{1}',phoneNumber='{2}'  WHERE email ='{3}';",name,surname,phoneNumber,email);
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


    }
}
