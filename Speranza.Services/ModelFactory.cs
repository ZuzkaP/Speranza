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

        public IUserForAdminModel CreateUserForAdminModel(IUser user)
        {
            UserForAdminModel model = new UserForAdminModel();
            model.Name = user.Name;
            model.Surname = user.Surname;
            model.Email = user.Email;
            model.PhoneNumber = user.PhoneNumber;

            return model;
        }
    }
}
