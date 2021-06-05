using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vkm.Api.Basic;
using Vkm.Api.Drawable;
using Vkm.Api.Identification;
using Vkm.Api.Layout;
using Vkm.Kernel;
using Vkm.TestProject.Entities;

namespace Vkm.TestProject
{
    [TestClass]
    public class DeviceManagerTests
    {
        [TestMethod]
        public void SetLayoutTest()
        {
            var initializer = new Initializer();
            var globalContext = initializer.GlobalContext;

            DeviceManager deviceManager = new DeviceManager(globalContext, initializer.Device); 
            
            TestLayout layout1 = new TestLayout(new Identifier("Layout.test1"));
            TestLayout layout2 = new TestLayout(new Identifier("Layout.test1"));
            
            Assert.AreEqual(false, layout1.LayoutEntered);
            Assert.AreEqual(true, layout1.LayoutLeaved);
            Assert.AreEqual(false, layout2.LayoutEntered);
            Assert.AreEqual(true, layout2.LayoutLeaved);
            
            deviceManager.SetLayout(layout1);
            Assert.AreEqual(true, layout1.LayoutEntered);
            Assert.AreEqual(false, layout1.LayoutLeaved);
            Assert.AreEqual(false, layout2.LayoutEntered);
            Assert.AreEqual(true, layout2.LayoutLeaved);

            deviceManager.SetLayout(layout2);
            Assert.AreEqual(false, layout1.LayoutEntered);
            Assert.AreEqual(true, layout1.LayoutLeaved);
            Assert.AreEqual(true, layout2.LayoutEntered);
            Assert.AreEqual(false, layout2.LayoutLeaved);

            deviceManager.SetPreviousLayout(layout2.Id);
            Assert.AreEqual(true, layout1.LayoutEntered);
            Assert.AreEqual(false, layout1.LayoutLeaved);
            Assert.AreEqual(false, layout2.LayoutEntered);
            Assert.AreEqual(true, layout2.LayoutLeaved);

            deviceManager.SetPreviousLayout(layout1.Id);
            Assert.AreEqual(false, layout1.LayoutEntered);
            Assert.AreEqual(true, layout1.LayoutLeaved);
            Assert.AreEqual(false, layout2.LayoutEntered);
            Assert.AreEqual(true, layout2.LayoutLeaved);
        }
        
        [TestMethod]
        public void DrawWorkflowTest()
        {
            var initializer = new Initializer();
            var globalContext = initializer.GlobalContext;

            DeviceManager deviceManager = new DeviceManager(globalContext, initializer.Device); 
            
            TestLayout layout1 = new TestLayout(new Identifier("Layout.test1"));
            TestLayout layout2 = new TestLayout(new Identifier("Layout.test1"));
            
            deviceManager.SetLayout(layout1);
            Assert.AreEqual(1, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            deviceManager.SetLayout(layout2);
            Assert.AreEqual(2, GetDrawnCycles(deviceManager, initializer.Device).Result);

            layout1.DoDraw(new Location(1, 1));
            Assert.AreEqual(2, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            layout2.DoDraw(new Location(1, 1));
            Assert.AreEqual(3, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            deviceManager.SetPreviousLayout();
            Assert.AreEqual(4, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            layout2.DoDraw(new Location(1, 1));
            Assert.AreEqual(4, GetDrawnCycles(deviceManager, initializer.Device).Result);

            layout1.DoDraw(new Location(1, 1));
            Assert.AreEqual(5, GetDrawnCycles(deviceManager, initializer.Device).Result);
        }
        
        [TestMethod]
        public void DrawWorkflowPauseTest()
        {
            var initializer = new Initializer();
            var globalContext = initializer.GlobalContext;

            DeviceManager deviceManager = new DeviceManager(globalContext, initializer.Device); 
            
            TestLayout layout1 = new TestLayout(new Identifier("Layout.test1"));
            TestLayout layout2 = new TestLayout(new Identifier("Layout.test1"));
            
            deviceManager.Pause();
            deviceManager.SetLayout(layout1);
            Assert.AreEqual(0, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            deviceManager.SetLayout(layout2);
            Assert.AreEqual(0, GetDrawnCycles(deviceManager, initializer.Device).Result);

            layout1.DoDraw(new Location(1, 1));
            Assert.AreEqual(0, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            layout2.DoDraw(new Location(1, 1));
            Assert.AreEqual(0, GetDrawnCycles(deviceManager, initializer.Device).Result);
            
            deviceManager.Resume();
            Assert.AreEqual(1, GetDrawnCycles(deviceManager, initializer.Device).Result);
        }
        
        [TestMethod]
        public void ButtonPressWorkflowTest()
        {
            var initializer = new Initializer();
            var globalContext = initializer.GlobalContext;

            DeviceManager deviceManager = new DeviceManager(globalContext, initializer.Device); 
            
            TestLayout layout1 = new TestLayout(new Identifier("Layout.test1"));
            TestLayout layout2 = new TestLayout(new Identifier("Layout.test1"));

            Tuple<Location, ButtonEvent> pressedButton1 = null;
            Tuple<Location, ButtonEvent> pressedButton2 = null;

            layout1.OnButtonPressed += (sender, tuple) => pressedButton1 = tuple;
            layout2.OnButtonPressed += (sender, tuple) => pressedButton2 = tuple;

            initializer.Device.PressButton(new Location(1, 1), true);
            Assert.AreEqual(null, pressedButton1);
            Assert.AreEqual(null, pressedButton2);
            initializer.Device.PressButton(new Location(1, 1), false);
            
            deviceManager.SetLayout(layout1);
            
            initializer.Device.PressButton(new Location(1, 1), true);
            Assert.AreEqual(new Location(1, 1), pressedButton1.Item1);
            Assert.AreEqual(ButtonEvent.Down, pressedButton1.Item2);
            Assert.AreEqual(null, pressedButton2);
            initializer.Device.PressButton(new Location(1, 1), false);
            Assert.AreEqual(new Location(1, 1), pressedButton1.Item1);
            Assert.AreEqual(ButtonEvent.Up, pressedButton1.Item2);
            Assert.AreEqual(null, pressedButton2);
            
            deviceManager.SetLayout(layout2);
            pressedButton1 = null;
            pressedButton2 = null;
            
            initializer.Device.PressButton(new Location(1, 1), true);
            Assert.AreEqual(new Location(1, 1), pressedButton2.Item1);
            Assert.AreEqual(ButtonEvent.Down, pressedButton2.Item2);
            Assert.AreEqual(null, pressedButton1);
            initializer.Device.PressButton(new Location(1, 1), false);
            Assert.AreEqual(new Location(1, 1), pressedButton2.Item1);
            Assert.AreEqual(ButtonEvent.Up, pressedButton2.Item2);
            Assert.AreEqual(null, pressedButton1);
            
            deviceManager.SetPreviousLayout();
            pressedButton1 = null;
            pressedButton2 = null;

            initializer.Device.PressButton(new Location(1, 1), true);
            Assert.AreEqual(new Location(1, 1), pressedButton1.Item1);
            Assert.AreEqual(ButtonEvent.Down, pressedButton1.Item2);
            Assert.AreEqual(null, pressedButton2);
            initializer.Device.PressButton(new Location(1, 1), false);
            Assert.AreEqual(new Location(1, 1), pressedButton1.Item1);
            Assert.AreEqual(ButtonEvent.Up, pressedButton1.Item2);
            Assert.AreEqual(null, pressedButton2);
            
            pressedButton1 = null;
            pressedButton2 = null;

            initializer.Device.PressButton(new Location(1, 1), true);
            Assert.AreEqual(ButtonEvent.Down, pressedButton1.Item2);
            Thread.Sleep(initializer.GlobalOptions.LongPressTimeout.Add(TimeSpan.FromSeconds(1)));
            Assert.AreEqual(ButtonEvent.LongPress, pressedButton1.Item2);
            initializer.Device.PressButton(new Location(1, 1), false);
            Assert.AreEqual(new Location(1, 1), pressedButton1.Item1);
            Assert.AreEqual(ButtonEvent.Up, pressedButton1.Item2);
        }

        async Task<int> GetDrawnCycles(DeviceManager deviceManager, TestDevice device)
        {
            while (!deviceManager.IsAllDrawn)
                await Task.Delay(50);

            return device.DrawnCycles;
        }
        
    }
}
