using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Database.Data;

namespace Speranza.Services.Interfaces
{
    public interface IModelFactory
    {
        IUserForAdminModel CreateUserForAdminModel(IUser user);
        ITrainingForAdminModel CreateTrainingForAdminModel(ITraining training);
        ITrainingModel CreateTrainingModel(ITraining training);
        IUserForTrainingDetailModel CreateUsersForTrainingDetailModel(IUser user);
        IRecurringTemplateModel CreateRecurringTrainingModel(IRecurringTrainingTemplate template);
        IUserProfileModel CreateUserForUserProfileModel(IUser user);
    }
}
