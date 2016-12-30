using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Speranza.Services;
using Speranza.Models.Interfaces;
using Moq;
using Speranza.Database.Data.Interfaces;

namespace Speranza.Tests.Services
{
    [TestClass]
    public class ModelFactoryShould
    {
        private ModelFactory factory;
        private Mock<IUser> user;
        private const string NAME = "Miro";
        private const string SURNAME = "Nejaký";
        private const string EMAIL = "test";
        private const string PHONENUMBER = "cislo";

        [TestMethod]
        public void CorrectlyCreateUserForAdminModel()
        {
            InitializeModelFactory();
            PrepareUserFromDatabase();

            IUserForAdminModel model = factory.CreateUserForAdminModel(user.Object);

            Assert.AreEqual(NAME, model.Name);
            Assert.AreEqual(SURNAME, model.Surname);
            Assert.AreEqual(EMAIL, model.Email);
            Assert.AreEqual(PHONENUMBER, model.PhoneNumber);

        }

        private void PrepareUserFromDatabase()
        {
            user = new Mock<IUser>();
            user.SetupGet(r => r.Name).Returns(NAME);
            user.SetupGet(r => r.Surname).Returns(SURNAME);
            user.SetupGet(r => r.Email).Returns(EMAIL);
            user.SetupGet(r => r.PhoneNumber).Returns(PHONENUMBER);
        }

        private void InitializeModelFactory()
        {
            factory = new ModelFactory();
        }
    }
}
