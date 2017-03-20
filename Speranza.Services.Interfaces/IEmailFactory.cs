﻿using System;
using Speranza.Smtp.Interfaces;

namespace Speranza.Services.Interfaces
{
    public interface IEmailFactory
    {
        Email CreateWelcomeEmail(string email, string welcomeSubject, string welcomeBody);
        Email CreateTrainingCanceledEmail(string email, string trainingCanceledSubject, string trainingCanceledBody, DateTime dateTime);
        Email CreateAddingUserToTrainingEmail(string email, string addingUserToTrainingSubject, string addingUserToTrainingBody, DateTime dateTime);
        Email CreateRemovingUserFromTrainingEmail(string email, string removingUserFromTrainingSubject, string removingUserFromTrainingBody, DateTime dateTime);
    }
}