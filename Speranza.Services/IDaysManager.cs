using Speranza.Models;
using System;

namespace Speranza.Services
{
    public interface IDaysManager
    {
        IDayModel GetDay(DateTime date);
    }
}