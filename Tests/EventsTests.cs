using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Events;
using System.Threading;

namespace Tests
{
    /// <summary>
    /// Summary description for EventsTests
    /// </summary>
    [TestClass]
    public class EventsTests
    {
        bool pass = false;
        Action<object> setPass;
        EventHandle<object> handle;

        public EventsTests()
        {
            setPass = (o) =>
            {
                pass = true;
            };
            handle = EventsBuiltin.RegisterEvent(EventID.TEST);
            EventsBuiltin.RegisterListener(EventID.TEST, setPass);
            handle.Notify();

            Thread.Sleep(10);
        }

        [TestMethod]
        public void RegisterRemove()
        {
            Assert.IsTrue(pass);
            pass = false;

            EventsBuiltin.RemoveListener(EventID.TEST, setPass);
            handle.Notify();

            Thread.Sleep(10);
            Assert.IsFalse(pass);
            pass = true;
        }

        [TestMethod]
        public void Reset()
        {
            int a = 0;
            Action b = () =>
            {
                a = 2;
            };
            Action<object> ba = (o) =>
            {
                b();
            };

            var h = EventsBuiltin.RegisterEvent(EventID.TEST2);
            EventsBuiltin.RegisterListener(EventID.TEST2, ba);

            h.Notify();
            Thread.Sleep(10);

            Assert.IsTrue(pass);
            Assert.AreEqual(2, a);

            b = () =>
            {
                a = 4;
            };
            h.Notify();
            Thread.Sleep(10);
            Assert.AreEqual(4, a);
        }
    }
}
