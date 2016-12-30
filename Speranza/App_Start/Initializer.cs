using Speranza.Database;
using Speranza.Services;
using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Speranza.App_Start
{
    public class Initializer
    {
        public static IDatabaseGateway Db { get;private set; }
        public static IHasher Hasher { get; private set; }
        public static IUserManager UserManager { get;private set; }
        public static ITrainingsManager TrainingsManager { get;private set; }
        public static IDateTimeService DateTimeService  { get;private set; }
        public static IDaysManager DaysManager { get; private set; }

        static  Initializer()
        {
            Db = InMemoryDatabase.Instance;
            Hasher = new Hasher();
            TrainingsManager = new TrainingsManager();
            DateTimeService = new DateTimeService();
            DaysManager = new DaysManager(Db,TrainingsManager,DateTimeService);
            var factory = new ModelFactory();
            UserManager = new UserManager(Db, factory);
        }
    }
}