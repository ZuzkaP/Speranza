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

        public string CreatePassword()
        {
            return Guid.NewGuid().ToString().Substring(0,8);
        }

        public string GenerateSeries()
        {
            return Guid.NewGuid().ToString();
        }

        public string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
