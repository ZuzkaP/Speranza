using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class UserDataParserShould
    {
        private const string INVALID_USER_DATA = "invalid";
        private const string VALID_USER_DATA = "valid ("+ EMAIL + ")";
        private const string EMAIL = "email";
        private UserDataParser parser;

        [TestMethod]
        public void ReturnNull_When_UserDataAreEmpty()
        {
            InitializeParser();

           string email = parser.ParseData(string.Empty);

            Assert.IsNull(email);
        }

        [TestMethod]
        public void ReturnNull_When_UserDataDoesNotContainEmail()
        {
            InitializeParser();

            string email = parser.ParseData(INVALID_USER_DATA);

            Assert.IsNull(email);
        }

        [TestMethod]
        public void ReturnEmail_When_UserDataAreCorrect()
        {
            InitializeParser();

            string email = parser.ParseData(VALID_USER_DATA);

            Assert.AreEqual(EMAIL,email);
        }

        private void InitializeParser()
        {
            parser = new UserDataParser();
        }
    }
}
