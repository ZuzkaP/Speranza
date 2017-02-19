﻿using System;
using Speranza.Database.Data.Interfaces;
using Speranza.Database.Data;
using Speranza.Models.Interfaces;
using Speranza.Services.Interfaces;
using Speranza.Models;
using System.Collections.Generic;
using Speranza.Database;
using Speranza.Services.Interfaces.Exceptions;

namespace Speranza.Services
{
    public class TrainingsManager: ITrainingsManager
    {
        private IDateTimeService dateTimeService;
        private IDatabaseGateway db;
        private IModelFactory factory;
        private IUidService uidService;
        private IUserManager userManager;

        public TrainingsManager(IDatabaseGateway db,IModelFactory factory, IUidService uidService, IDateTimeService dateTimeService, IUserManager userManager)
        {
            this.db = db;
            this.factory = factory;
            this.uidService = uidService;
            this.dateTimeService = dateTimeService;
            this.userManager = userManager;
        }

        public CalendarMessages AddUserToTraining(string email, string trainingID, DateTime currentDate)
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

            return CalendarMessages.SignUpSuccessful;
        }

        public void CancelTraining(string trainingID)
        {
            db.CancelTraining(trainingID);
        }

        public string CreateNewTraining(DateTime dateTime, string trainer, string description, int capacity)
        {
            string trainingID = uidService.CreateID();
            db.CreateNewTraining(trainingID, dateTime, trainer, description, capacity);

            return trainingID;
        }

        public void CreateRecurringTraining(IRecurringModel model)
        {
            int day = 0;
            int time = 7; 
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

                    db.CreateRecurringTrainingTemplate(template);
                }
                time++;
                if(time > 19)
                {
                    day++;
                    time = 7;
                }
            }
        }

        public ITrainingModel GenerateTrainingFromTemplate(IRecurringTrainingTemplate template)
        {
            throw new NotImplementedException();
        }

        public IList<ITrainingForAdminModel> GetAllFutureTrainings()
        {
            var trainingsFromDB = db.GetAllTrainings();
            var trainingsForAdmin = new List<ITrainingForAdminModel>();
            foreach (var item in trainingsFromDB)
            {
               if(item.Time > dateTimeService.GetCurrentDate())
                {
                    ITrainingForAdminModel model = factory.CreateTrainingForAdminModel(item);
                    trainingsForAdmin.Add(model);
                }
            }
            return trainingsForAdmin;
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
        }

        public ITrainingModel RemoveUserFromTraining(string email, string id)
        {
            db.RemoveUserFromTraining(email, id);
            ITraining training = db.GetTrainingData(id);
            ITrainingModel model = factory.CreateTrainingModel(training);

            return model;
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
    }
}