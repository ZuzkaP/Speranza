using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Speranza.Database;
using Speranza.Database.Data.Interfaces;
using Speranza.Services;
using Speranza.Services.Interfaces;

namespace Speranza.Tests.Controllers
{
    public class MessageManagerShould
    {
        private MessageManager manager;
        private Mock<IDatabaseGateway> db;
        private const string MESSAGE = "message";

        private void InitializeMessageManager()
        {
            db = new Mock<IDatabaseGateway>();
            manager = new MessageManager(db.Object);
        }


        [TestMethod]
        public void AddNewMessage()
        {
            InitializeMessageManager();

            manager.AddNewInfoMessage(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3), MESSAGE);

            db.Verify(r => r.AddNewMessage(DateTime.Today.AddDays(2), DateTime.Today.AddDays(3), MESSAGE), Times.Once);
        }


        [TestMethod]
        public void GetNoMessageFromDBIFDoesNotExistInCurrentDate()
        {
            InitializeMessageManager();
            db.Setup(r => r.GetMessageForCurrentDate()).Returns(String.Empty);
            string result = manager.GetMessageForCurrentDate();

            Assert.IsNull(result);
            db.Verify(r => r.GetMessageForCurrentDate(), Times.Once);
        }


        [TestMethod]
        public void GetMessageFromDBIfExistsInCurrentDate()
        {
            InitializeMessageManager();
            db.Setup(r => r.GetMessageForCurrentDate()).Returns(MESSAGE);
            string result = manager.GetMessageForCurrentDate();

            Assert.AreEqual(MESSAGE, result);
            db.Verify(r => r.GetMessageForCurrentDate(), Times.Once);
        }
    }
}
