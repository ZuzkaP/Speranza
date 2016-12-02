using Speranza.Models;
using Speranza.Models.Interfaces;
using System;

namespace Speranza.Services.Interfaces
{
    public interface IDaysManager
    {
        IDayModel GetDay(DateTime date);
    }
}