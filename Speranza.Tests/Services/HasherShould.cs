using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Models;
using Speranza.Services;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class HasherShould
    {
        private Hasher hasher;

        private void InitializeHasher()
        {
            hasher = new Hasher();
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NotHashWhenPassIsNull()
        {
            InitializeHasher();

            string hashOfEmptyString1 = hasher.HashPassword(null);
        }


        [TestMethod]
        public void HashEmptyPassword()
        {
            InitializeHasher();
            
            string hashOfEmptyString1 = hasher.HashPassword(string.Empty);
            string hashOfEmptyString2 = hasher.HashPassword(string.Empty);

            Assert.AreEqual(hashOfEmptyString1, hashOfEmptyString2);

        }

        [TestMethod]
        public void HastheSamePasswords()
        {
            InitializeHasher();

            string hashOfEmptyString1 = hasher.HashPassword("pass1");
            string hashOfEmptyString2 = hasher.HashPassword("pass1");

            Assert.AreEqual(hashOfEmptyString1, hashOfEmptyString2);
        }

        [TestMethod]
        public void HashDifferentPasswords()
        {
            InitializeHasher();

            string hashOfEmptyString1 = hasher.HashPassword("pass1");
            string hashOfEmptyString2 = hasher.HashPassword("pass2");

            Assert.AreNotEqual(hashOfEmptyString1, hashOfEmptyString2);
            
        }
    }
}
