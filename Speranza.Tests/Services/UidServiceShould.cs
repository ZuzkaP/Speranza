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

        private void InitializeUidService()
        {
            uidService = new UidService();
        }
    }
}
