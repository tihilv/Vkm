using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.Api.Basic;

namespace Vkm.TestProject
{
    [TestClass]
    public class DisposeHelperTests
    {
        [TestMethod]
        public void TestDisposeHelper()
        {
            Test test = new Test();
            Test testCopy = test;
            Assert.AreEqual(false, test.Disposed, "Object already disposed");
            
            DisposeHelper.DisposeAndNull(ref test);
            
            Assert.AreEqual(true, testCopy.Disposed, "Object not disposed");
            Assert.AreEqual(null, test, "Object not cleared");
        }

        class Test : IDisposable
        {
            private bool _disposed;

            public bool Disposed => _disposed;

            public void Dispose()
            {
                _disposed = true;
            }
        }
    }
}