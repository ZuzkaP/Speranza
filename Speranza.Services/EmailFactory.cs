using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Speranza.Smtp.Interfaces;

namespace Speranza.Services
{
    public class EmailFactory : IEmailFactory
    {
        public Email CreateWelcomeEmail(string email, string welcomeSubject, string welcomeBody)
        {
            throw new NotImplementedException();
        }
    }
}
