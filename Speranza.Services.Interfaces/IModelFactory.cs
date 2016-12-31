using Speranza.Database.Data.Interfaces;
using Speranza.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services.Interfaces
{
    public interface IModelFactory
    {
        IUserForAdminModel CreateUserForAdminModel(IUser user);
        ITrainingForAdminModel CreateTrainingForAdminModel(ITraining training);
    }
}
