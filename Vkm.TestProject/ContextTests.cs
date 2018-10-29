using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.TestProject.Entities;

namespace Vkm.TestProject
{
    [TestClass]
    public class ContextTests
    {
        [TestMethod]
        public void ServiceTest()
        {
            var initializer = new Initializer();
            var globalContext = initializer.GlobalContext;
            var service = globalContext.GetServices<ITestService>().Single();
            Assert.AreEqual(2, service.GetValue(1));
            Assert.AreEqual(0, service.InitContextId);
            Assert.AreEqual(1, service.InitOptionsId);
            Assert.AreEqual(2, service.InitId);
            Assert.AreEqual(service.CreatedOptions, service.AssignedOptions);
            
            var serviceCopy = globalContext.GetServices<ITestService>().Single();
            Assert.AreEqual(2, service.GetValue(1));
            Assert.AreEqual(0, service.InitContextId);
            Assert.AreEqual(1, service.InitOptionsId);
            Assert.AreEqual(2, service.InitId);
            Assert.AreEqual(service, serviceCopy);
            Assert.AreEqual(service.CreatedOptions, service.AssignedOptions);
        }
    }
}