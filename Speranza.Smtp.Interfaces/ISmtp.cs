using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Smtp.Interfaces
{
    public interface ISmtp
    {
        void SendEmail(Email email);
    }
}
