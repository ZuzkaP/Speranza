using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Smtp.Interfaces
{
   public class Email
    {
        public string Receiver { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


    }
}
