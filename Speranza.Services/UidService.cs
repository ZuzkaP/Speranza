using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services
{
    public class UidService : IUidService
    {
        public string CreateID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
