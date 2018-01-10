using Speranza.Database;
using Speranza.Services;
using Speranza.Services.Interfaces;
using Speranza.Smtp;
using Speranza.Smtp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Speranza.Models;

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
        public static ModelFactory Factory { get; private set; }
        public static IUserDataParser UserDataParser { get; private set; }
        public static IEmailManager EmailManager { get; private set; }
        public static IEmailFactory EmailFactory { get; private set; }
        public static ICookieService CookieService { get; private set; }
        public static ISmtp Smtp { get; private set; }
        public static IUidService UidService { get; private set; }
        public static IGalleryService GalleryService { get; private set; }

        static  Initializer()
        {
            Db = InMemoryDatabase.Instance;
            //Db = new Database.Database();
            Hasher = new Hasher();
            DateTimeService = new DateTimeService();
            Factory = new ModelFactory();
            EmailFactory = new EmailFactory();
            Smtp = new Smtp.Smtp();
            UidService = new UidService();
            UserDataParser = new UserDataParser();
            EmailManager = new EmailManager(EmailFactory, Smtp);
            UserManager = new UserManager(Db, Factory,DateTimeService,Hasher,EmailManager, UidService);
            TrainingsManager = new TrainingsManager(Db, Factory, UidService, DateTimeService, UserManager, EmailManager);
            DaysManager = new DaysManager(Db, TrainingsManager, DateTimeService, Factory);
            CookieService = new CookieService();
            GalleryService = new GalleryService();
        }
    }
}