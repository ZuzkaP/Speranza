using Speranza.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Services
{
    public class UserDataParser : IUserDataParser
    {
        public string ParseData(string userData)
        {
            if(string.IsNullOrEmpty(userData))
            {
                return null;
            }
            int indexOfFirstBracket = userData.LastIndexOf("(");
            int indexOfLastBracket = userData.LastIndexOf(")");
            if(indexOfFirstBracket <0 || indexOfLastBracket <0)
            {
                return null;
            }
            string email = userData.Substring(indexOfFirstBracket + 1, (indexOfLastBracket - indexOfFirstBracket)-1);

            return email;
        }
    }
}
