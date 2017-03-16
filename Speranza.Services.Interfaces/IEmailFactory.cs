using System;
using Speranza.Smtp.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface IEmailFactory
    {
        Email CreateWelcomeEmail(string email, string welcomeSubject, string welcomeBody);
        Email CreateTrainingCanceledEmail(string email, string trainingCanceledSubject, string trainingCanceledBody, DateTime dateTime);
    }
}