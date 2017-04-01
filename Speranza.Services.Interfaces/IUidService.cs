using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services.Interfaces
{
    public interface IUidService
    {
        string CreateID();
        string CreatePassword();
    }
}
