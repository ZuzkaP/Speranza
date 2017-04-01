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
   public class UidServiceShould
    {
        private UidService uidService;

        [TestMethod]
        public void CreateID()
        {
            InitializeUidService();

            string id = uidService.CreateID();

            Assert.IsNotNull(id);
            Assert.AreEqual(36, id.Length);

        }

        [TestMethod]
        public void CreatePass()
        {
            InitializeUidService();

            string pass = uidService.CreatePassword();

            Assert.IsNotNull(pass);
            Assert.AreEqual(8, pass.Length);

        }

        private void InitializeUidService()
        {
            uidService = new UidService();
        }
    }
}
