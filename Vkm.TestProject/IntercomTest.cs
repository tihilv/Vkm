using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.Intercom;
using Vkm.Intercom.Channels;
using Vkm.Intercom.Dispatchers;

namespace Vkm.TestProject
{
    [TestClass]
    public class IntercomTest
    {
        [TestMethod]
        public void TestChannels()
        {
            string pipeName = "Vkm.TestPipe1";

            var service = new Test1();
            IntercomClientChannel<Test1> clientChannel = new IntercomClientChannel<Test1>(pipeName);
            var task = clientChannel.ConnectAsync();
            IntercomServerChannel<Test1> serverChannel = new IntercomServerChannel<Test1>(service, pipeName);
            task.Wait();

            Assert.AreEqual(0, service.MethodCallCount);
            var r = clientChannel.Execute(nameof(Test1.Do), 5, 7);
            Assert.AreEqual(1, service.MethodCallCount);
            Assert.AreEqual(5 * 7, r);
            
            r = clientChannel.Execute(nameof(Test1.Do), 11, -11);
            Assert.AreEqual(2, service.MethodCallCount);
            Assert.AreEqual(-11*11, r);
        }

        [TestMethod]
        public void TestChannelDispose()
        {
            string pipeName = "Vkm.TestPipe2";

            var service = new Test1();
            IntercomServerChannel<Test1> serverChannel = new IntercomServerChannel<Test1>(service, pipeName);
            IntercomClientChannel<Test1> clientChannel = new IntercomClientChannel<Test1>(pipeName);
            clientChannel.Connect();
            
            object disposed = null;
            serverChannel.ConnectionClosed += (sender, args) => disposed = new object();
            
            Assert.AreEqual(null, disposed);
            clientChannel.Dispose();
            Thread.Sleep(500);
            Assert.AreNotEqual(null, disposed);
        }

        [TestMethod]
        public void TestDuplexChannels()
        {
            string pipeName = "Vkm.TestPipe3";

            var service = new Test1();
            var callback = new Test2();

            var master = new IntercomDuplexChannel<Test1, Test2>(service, pipeName, true);
            var slave = new IntercomDuplexChannel<Test2, Test1>(callback, pipeName, false);
            
            Assert.AreEqual(0, service.MethodCallCount);
            var r = slave.Execute(nameof(Test1.Do), 5, 1);
            Assert.AreEqual(1, service.MethodCallCount);
            Assert.AreEqual(5, r);

            Assert.AreEqual(0, callback.MethodCallCount);
            r = master.Execute(nameof(Test2.Do));
            Assert.AreEqual(1, callback.MethodCallCount);
            Assert.AreEqual(string.Empty, r);
        }

        [TestMethod]
        public void TestDispatching()
        {
            string pipeName = "Vkm.TestPipe4";

            Test3.Counter = 0;
            IntercomMasterDispatcher<Test3, Test2> masterDispatcher = new IntercomMasterDispatcher<Test3, Test2>(pipeName, () => new Test3());
            
            var callback = new Test2();
            var connectedClient1 = IntercomSlaveDispatcher<Test3, Test2>.CreateSlaveChannel(callback, pipeName);
            Assert.AreEqual(1, Test3.Counter);
            var connectedClient2 = IntercomSlaveDispatcher<Test3, Test2>.CreateSlaveChannel(callback, pipeName);
            Assert.AreEqual(2, Test3.Counter);

            Assert.AreEqual(0, connectedClient1.Execute(nameof(Test3.GetInstanceIndex)));
            Assert.AreEqual(1, connectedClient2.Execute(nameof(Test3.GetInstanceIndex)));
            
            connectedClient1.Dispose();
            Thread.Sleep(500);
            Assert.AreEqual(1, Test3.Counter);
            
            masterDispatcher.Dispose();
            Assert.AreEqual(0, Test3.Counter);
        }

        [TestMethod]
        public void TestOneWay()
        {
            string pipeName = "Vkm.TestPipe5";

            IntercomMasterDispatcher<Test4, Test2> masterDispatcher = new IntercomMasterDispatcher<Test4, Test2>(pipeName, () => new Test4());
            
            var callback = new Test2();
            var connectedClient = IntercomSlaveDispatcher<Test4, Test2>.CreateSlaveChannel(callback, pipeName);

            Assert.AreEqual(false, Test4.Success);
            connectedClient.Execute(nameof(Test4.LongMethod));
            Assert.AreEqual(false, Test4.Success);
            Thread.Sleep(1100);
            Assert.AreEqual(true, Test4.Success);
        }

        

        class Test1 : IRemoteService
        {
            internal static int Counter;

            private readonly int _instanceIndex;
            private int _methodCallCount;

            public int InstanceIndex => _instanceIndex;

            public int MethodCallCount => _methodCallCount;

            public Test1()
            {
                _instanceIndex = Counter++;
            }

            public int Do(int c, int d)
            {
                _methodCallCount++;

                return c * d;
            }
        }

        class Test2 : IRemoteService
        {
            internal static int Counter;
            
            private readonly int _instanceIndex;
            private int _methodCallCount;

            public int InstanceIndex => _instanceIndex;

            public int MethodCallCount => _methodCallCount;

            public Test2()
            {
                _instanceIndex = Counter++;
            }
            
            public string Do()
            {
                _methodCallCount++;

                return string.Empty;
            }
        }
        
        class Test3 : Test1, IDisposable
        {
            private bool _disposed;

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    Counter--;
                }
            }

            public int GetInstanceIndex()
            {
                return InstanceIndex;
            }
        }
        
        class Test4 : Test1
        {
            private static bool _success;
            
            public static bool Success => _success;

            [OneWay]
            public void LongMethod()
            {
                _success = false;
                Thread.Sleep(1000);
                _success = true;
            }
        }
    }
}