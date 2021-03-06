﻿using System;
using Speranza.Database.Data.Interfaces;
using Speranza.Database.Data;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Models;
using System.Collections.Generic;
using Speranza.Database;
using Speranza.Services.Interfaces.Exceptions;
using System.Linq;

namespace Speranza.Services
{
    public class TrainingsManager: ITrainingsManager
    {
        private IDateTimeService dateTimeService;
        private IDatabaseGateway db;
        private IModelFactory factory;
        private IUidService uidService;
        private IUserManager userManager;
        private IEmailManager emailManager;

        public TrainingsManager(IDatabaseGateway db,IModelFactory factory, IUidService uidService, IDateTimeService dateTimeService, IUserManager userManager, IEmailManager emailManager)
        {
            this.db = db;
            this.factory = factory;
            this.uidService = uidService;
            this.dateTimeService = dateTimeService;
            this.userManager = userManager;
            this.emailManager = emailManager;
        }

        public CalendarMessages AddUserToTraining(string email, string trainingID, DateTime currentDate, bool isAdmin)
        {
            if(!userManager.UserExists(email))
             {
                return CalendarMessages.UserDoesNotExist;
             }
            var training = db.GetTrainingData(trainingID);
            if ( training == null)
            {
                return CalendarMessages.TrainingDoesNotExist;
            }
            if(training.Capacity <= training.RegisteredNumber)
            {
                return CalendarMessages.TrainingIsFull;
            }
            if(db.IsUserAlreadySignedUpInTraining(email,trainingID))
            {
               return CalendarMessages.UserAlreadySignedUp;
            }
            db.AddUserToTraining(email, trainingID, currentDate);

            if(isAdmin)
            {
                emailManager.SendAddingUserToTraining(email, training.Time);
            }
            else if(training.RegisteredNumber == 5)
            {
                emailManager.SendSixthUserInTraining(db.GetAdmins(), training.Time);
            }
            return CalendarMessages.SignUpSuccessful;
        }


        public CalendarMessages AddUserToTraining(string email, string trainingID, DateTime currentDate)
        {
            return AddUserToTraining(email, trainingID, currentDate, false);
        }

        public void CancelTraining(string trainingID)
        {
            var emails = db.GetEmailsOfAllUsersInTraining(trainingID);
            var training = db.GetTrainingData(trainingID);
            db.CancelTraining(trainingID);

            if(emails != null)
            {
                foreach (var item in emails)
                {
                    emailManager.SendTrainingCanceled(item, training.Time);
                }
            }
        }

        public string CreateNewTraining(DateTime dateTime, string trainer, string description, int capacity)
        {
           return  CreateNewTraining(dateTime, trainer, description, capacity, false);
        }

        public string CreateNewTraining(DateTime dateTime, string trainer, string description, int capacity, bool isFromTemplate)
        {
            string trainingID = uidService.CreateID();
            db.CreateNewTraining(trainingID, dateTime, trainer, description, capacity,isFromTemplate);

            return trainingID;
        }

        public void CreateRecurringTraining(IRecurringModel model)
        {
            int day = 0;
            int time = 6; 
             foreach (var item in model.IsTrainingInTime)
            {
                if (item)
                {
                    RecurringTrainingTemplate template = new RecurringTrainingTemplate();
                    template.Capacity = model.Capacity;
                    template.Description = model.Description;
                    template.Trainer = model.Trainer;
                    template.Day = day;
                    template.Time = time;
                    DateTime tempValidFrom;
                    DateTime.TryParse(model.ValidFrom,out tempValidFrom);
                    template.ValidFrom = tempValidFrom;

                    db.CreateRecurringTrainingTemplate(template);
                    db.SetLastTemplateGenerationDate(dateTimeService.GetCurrentDateTime().AddDays(-1));
                }
                time++;
                if(time > 20)
                {
                    day++;
                    time = 6;
                }
            }
        }

        public ITrainingModel GenerateTrainingFromTemplate(IRecurringTrainingTemplate template, DateTime date)
        {
            DateTime trainingDate = new DateTime(date.Year, date.Month, date.Day, template.Time, 00, 00);
            var trainingID = CreateNewTraining(trainingDate, template.Trainer, template.Description, template.Capacity,true);
            ITraining training = db.GetTrainingData(trainingID);
            return factory.CreateTrainingModel(training);
        }

        public IList<ITrainingForAdminModel> GetFutureTrainings(int from, int to)
        {
            var trainingsFromDB = db.GetAllTrainings();
            var trainingsForAdmin = new List<ITrainingForAdminModel>();
            foreach (var item in trainingsFromDB)
            {
               if(item.Time > dateTimeService.GetCurrentDateTime())
                {
                    ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(item);
                    trainingsForAdmin.Add(model);
                }
            }
            return trainingsForAdmin.OrderBy(r=>r.Time).Skip(from).Take(to - from).ToList();
        }

        public IList<ITrainingForAdminModel> GetFutureTrainings(DateTime date)
        {
            var trainingsFromDB = db.GetAllTrainings();
            var trainingsForAdmin = new List<ITrainingForAdminModel>();
            foreach (var item in trainingsFromDB)
            {
                if (item.Time.Date == date.Date)
                {
                    if (date.Date == dateTimeService.GetCurrentDate().Date && item.Time < dateTimeService.GetCurrentDateTime())
                    {
                        continue;
                    }
                    ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(item);
                    trainingsForAdmin.Add(model);
                }
            }
            return trainingsForAdmin.OrderBy(r => r.Time).ToList();
        }

        public IList<ITrainingModel> GetFutureTrainings()
        {
            var trainingsFromDB = db.GetAllTrainings().Where(r=>r.Time >= dateTimeService.GetCurrentDateTime());
            var trainingsForAdmin = new List<ITrainingModel>();
            foreach (var item in trainingsFromDB)
            {
                ITrainingModel model = factory.CreateTrainingModel(item);
                trainingsForAdmin.Add(model);
            }
            return trainingsForAdmin.OrderBy(r => r.Time).ToList();
        }

        public IList<IUserForTrainingDetailModel> GetAllUsersInTraining(string trainingID)
        {
           IList<IUser> users = db.GetUsersInTraining(trainingID);
            IList<IUserForTrainingDetailModel> models = new List<IUserForTrainingDetailModel>();
            foreach (var item in users)
            {
                IUserForTrainingDetailModel model = factory.CreateUsersForTrainingDetailModel(item);
                models.Add(model);
            }
            return models;
        }

        public int GetSignOffLimit()
        {
            return db.GetSignOffLimit();
        }

        public IList<IRecurringTemplateModel> GetTemplates()
        {
            var templates = db.GetTemplates();
            var list = new List<IRecurringTemplateModel>();
            foreach (var item in templates)
            {
                IRecurringTemplateModel model = factory.CreateRecurringTrainingModel(item);
                list.Add(model);
            }
            return list;
        }

        public void RemoveTrainingTemplate(int day, int time)
        {
            db.RemoveTrainingTemplate(day, time);
            db.SetLastTemplateGenerationDate(dateTimeService.GetCurrentDateTime().AddDays(-1));
        }

        public ITrainingModel RemoveUserFromTraining(string email, string id,bool isAdmin)
        {
            ITraining training = db.GetTrainingData(id);
            db.RemoveUserFromTraining(email, id);
            if(isAdmin)
            {
            emailManager.SendRemovingUserFromTraining(email, training.Time);
            }
            else if(training.RegisteredNumber == 6)
            {
                var admins = db.GetAdmins();
                emailManager.SendSixthUserSignOffFromTraining(admins, training.Time);
            }
            ITrainingModel model = factory.CreateTrainingModel(training);

            return model;
        }

        public ITrainingModel RemoveUserFromTraining(string email, string id)
        {
           return RemoveUserFromTraining(email, id, false);
        }

        public void SetSignOffLimit(int hoursLimit)
        {
            db.SetSignOffLimit(hoursLimit);
        }

        public void SetTrainer(string trainingID, string trainer)
        {
            db.SetTrainer(trainingID, trainer);
        }

        public void SetTrainingCapacity(string trainingID, int capacity)
        {
            db.SetTrainingCapacity(trainingID, capacity);
        }

        public void SetTrainingDescription(string trainingID, string trainingDescription)
        {
            db.SetTrainingDescription(trainingID, trainingDescription);
        }

        public int GetFutureTrainingsCount()
        {
            return db.GetTrainingsCountAfterDate(dateTimeService.GetCurrentDateTime());
        }

        public IList<ITrainingForAdminModel> GetPastTrainings(int from, int to)
        {
            var trainingsFromDB = db.GetAllTrainings();
            var trainingsForAdmin = new List<ITrainingForAdminModel>();
            foreach (var item in trainingsFromDB)
            {
                if (item.Time <= dateTimeService.GetCurrentDateTime())
                {
                    ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(item);
                    trainingsForAdmin.Add(model);
                }
            }
            return trainingsForAdmin.OrderByDescending(r => r.Time).Skip(from).Take(to - from).ToList();
        }

        public IList<ITrainingForAdminModel> GetPastTrainings(DateTime date)
        {
            var trainingsFromDB = db.GetAllTrainings();
            var trainingsForAdmin = new List<ITrainingForAdminModel>();
            foreach (var item in trainingsFromDB)
            {
                if (item.Time.Date == date.Date)
                {
                    if (date.Date == dateTimeService.GetCurrentDate().Date && item.Time > dateTimeService.GetCurrentDateTime())
                    {
                        continue;
                    }
                    ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(item);
                    trainingsForAdmin.Add(model);
                }
            }
            return trainingsForAdmin.OrderByDescending(r => r.Time).ToList();
        }

        public int GetPastTrainingsCount()
        {
            return db.GetTrainingsCountBeforeDate(dateTimeService.GetCurrentDateTime());
        }

        public void ConfirmParticipation(string trainingID, string email)
        {
            db.ConfirmParticipation(trainingID, email);
            db.AllowSigningUpToTrainings(email);
        }

        public void DisproveParticipation(string trainingID, string email)
        {
            db.DisproveParticipation(trainingID, email);
            db.SignOutUserFromAllTrainingsAfterDate(email,dateTimeService.GetCurrentDateTime());
            db.ForbidSigningUpToTrainings(email);
        }
    }
}