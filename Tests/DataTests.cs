using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Drawing;
using Data;

namespace Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DataTests
    {
        public DataTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        [TestMethod]
        public void AngleTests()
        {
            Assert.AreEqual(Math.PI, Angle.PI);
            Assert.AreEqual(Math.PI * 3 /4, Angle.PI * (3f/4f));
            Assert.AreEqual(Math.PI / -4, Angle.PI / -4f);
        }

        [TestMethod]
        public void TrigAngleBetweenPoints()
        {
            Point a = new Point(1, 1), b = new Point(1, 2);
            Assert.AreEqual(Angle.PI/2, Trig.AngleBetween(a, b));

            a = new Point(1, 1);
            b = new Point(2, 2);
            Assert.AreEqual(Angle.PI/4, Trig.AngleBetween(a, b));

            a = new Point(1, 1);
            b = new Point(0, 2);
            Angle expected = Angle.PI * (3f / 4f);
            Console.WriteLine("{0}, {1}", expected, Trig.AngleBetween(a, b));
            Assert.IsTrue(Math.Abs((float)(expected - Trig.AngleBetween(a, b))) < 0.001f);

            a = new Point(1, 1);
            b = new Point(2, 0);
            Assert.AreEqual(Angle.PI / -4f, Trig.AngleBetween(a, b));
        }
    }
}
