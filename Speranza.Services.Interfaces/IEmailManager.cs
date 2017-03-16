using System;

namespace Speranza.Services.Interfaces
{
    public interface IEmailManager
    {
        void SendWelcome(string email);
        void SendTrainingCanceled(string email, DateTime dateTime);
    }
}