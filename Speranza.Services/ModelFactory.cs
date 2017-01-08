using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using Speranza.Models;

namespace Speranza.Services
{
    public class ModelFactory : IModelFactory
    {
        public ITrainingForAdminModel CreateTrainingForAdminModel(ITraining training)
        {
            ITrainingForAdminModel model = new TrainingForAdminModel();
            model.Capacity = training.Capacity;
            model.Description = training.Description;
            model.ID = training.ID;
            model.RegisteredNumber = training.RegisteredNumber;
            model.Time = training.Time;
            model.Trainer = training.Trainer;

            return model;
        }

        public ITrainingModel CreateTrainingModel(ITraining training)
        {
            ITrainingModel model = new TrainingModel(training.ID);

            model.Capacity = training.Capacity;
            model.Description = training.Description;
            model.RegisteredNumber = training.RegisteredNumber;
            model.Time = training.Time;
            model.Trainer = training.Trainer;
            //not signed up by default
            model.IsUserSignedUp = false;

            return model;
        }

        public IUserForAdminModel CreateUserForAdminModel(IUser user)
        {
            UserForAdminModel model = new UserForAdminModel();
            model.Name = user.Name;
            model.Surname = user.Surname;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;
            model.IsAdmin = user.IsAdmin;
            model.Category = user.Category.ToString();
            model.NumberOfFreeSignUps = user.NumberOfFreeSignUpsOnSeasonTicket;
            model.TrainingCount = user.NumberOfSignedUpTrainings;

            return model;
        }

        public IUserForTrainingDetailModel CreateUsersForTrainingDetailModel(IUser user)
        {
            IUserForTrainingDetailModel model = new UserForTrainingDetailModel();
            model.Name = user.Name;
            model.Surname = user.Surname;
            model.Email = user.Email;

            return model;
        }
    }
}
