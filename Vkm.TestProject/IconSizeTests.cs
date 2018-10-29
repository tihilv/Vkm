using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.Api.Basic;

namespace Vkm.TestProject
{
    [TestClass]
    public class IconSizeTests
    {
        [TestMethod]
        public void IconSizeTest()
        {
            IconSize iconSize = new IconSize(1, 2);
            Assert.AreEqual(1, iconSize.Width);
            Assert.AreEqual(2, iconSize.Height);
        }
        
        [TestMethod]
        public void LocationEqualityTest()
        {
            IconSize item1 = new IconSize(1, 2);
            IconSize item2 = new IconSize(2, 2);
            IconSize item3 = new IconSize(1, 2);
            IconSize item4 = new IconSize(1, 3);
            
            Assert.AreEqual(item1, item3);
            Assert.AreNotEqual(item1, item2);
            Assert.AreNotEqual(item1, item4);
        }
    }
}