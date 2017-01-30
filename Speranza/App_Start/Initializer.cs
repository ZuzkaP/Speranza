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
        private static UidService uidService;

        public static IDatabaseGateway Db { get;private set; }
        public static IHasher Hasher { get; private set; }
        public static IUserManager UserManager { get;private set; }
        public static ITrainingsManager TrainingsManager { get;private set; }
        public static IDateTimeService DateTimeService  { get;private set; }
        public static IDaysManager DaysManager { get; private set; }
        public static ModelFactory Factory { get; private set; }
        public static IUserDataParser UserDataParser { get; private set; }

        static  Initializer()
        {
            Db = InMemoryDatabase.Instance;
            Hasher = new Hasher();
            DateTimeService = new DateTimeService();
            Factory = new ModelFactory();
            uidService = new UidService();
            UserDataParser = new UserDataParser();
            DaysManager = new DaysManager(Db, TrainingsManager, DateTimeService,Factory);
            UserManager = new UserManager(Db, Factory,DateTimeService);
            TrainingsManager = new TrainingsManager(Db, Factory, uidService, DateTimeService, UserManager);
        }
    }
}