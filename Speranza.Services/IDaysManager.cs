using Speranza.Models;
using Speranza.Models.Interfaces;
using System;

namespace Speranza.Services
{
    public interface IDaysManager
    {
        IDayModel GetDay(DateTime date);
    }
}