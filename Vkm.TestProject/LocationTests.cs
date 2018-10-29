using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.Api.Basic;

namespace Vkm.TestProject
{
    [TestClass]
    public class LocationTests
    {
        [TestMethod]
        public void LocationAddTest()
        {
            Location location = new Location(1, 2);
            Assert.AreEqual(1, location.X);
            Assert.AreEqual(2, location.Y);
            
            Location locationToAdd = new Location(6, 12);
            Location sum = location + locationToAdd;
            
            Assert.AreEqual(7, sum.X);
            Assert.AreEqual(14, sum.Y);
        }
        
        [TestMethod]
        public void LocationEqualityTest()
        {
            Location location1 = new Location(1, 2);
            Location location2 = new Location(2, 2);
            Location location3 = new Location(1, 2);
            Location location4 = new Location(1, 3);
            
            Assert.AreEqual(location1, location3);
            Assert.AreNotEqual(location1, location2);
            Assert.AreNotEqual(location1, location4);
        }
    }
}