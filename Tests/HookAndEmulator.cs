using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InputHook;
using System.Threading;
using System.Windows.Forms;
using WindowsInput.Native;
using WindowsInput;
using OutputSimulator;
using System.Windows.Input;

namespace Tests
{
    [TestClass]
    public class HookAndEmulator
    {
        const int w = 500;

        VirtualMouse m = new VirtualMouse();
        IKeyboardSimulator ks = new KeyboardSimulator(new InputSimulator());

        [TestMethod]
        public void MouseMove()
        {
            const int x = 12;
            const int y = 456;

            m.Move(x, y);

            Assert.AreEqual(x, Cursor.Position.X);
            Assert.AreEqual(y, Cursor.Position.Y);
        }

        [TestMethod]
        public void MouseClick()
        {
            const int x = 12;
            const int y = 456;

            bool down = false, up = false;
            Hook.I().AddMouseHook((MouseAction a, int mx, int my) =>
            {
                if (a == MouseAction.WM_LBUTTONDOWN)
                    down = mx == x && my == y;
                if (a == MouseAction.WM_LBUTTONUP)
                    up = mx == x && my == y;

                Console.WriteLine("{0} at {1}, {2}", a.ToString(), mx, my);
            });

            Thread.Sleep(w * 3);
            m.DoAction(MouseAction.WM_LBUTTONDOWN, x, y);
            m.DoAction(MouseAction.WM_LBUTTONUP, x, y);
            Thread.Sleep(w * 3);
            Assert.IsTrue(up && down);
        }

        [TestMethod]
        public void KeyX()
        {
            bool down = false, up = false;
            Hook.I().AddKeyHook((Key k) =>
            {
                if(k == Key.X)
                {
                    down = true;
                }

                Console.WriteLine("d");
            }, (Key k) =>
            {
                if (k == Key.X)
                {
                    up = true;
                }

                Console.WriteLine("u");
            });

            Thread.Sleep(w);
            ks.KeyPress(VirtualKeyCode.VK_X);
            Assert.IsTrue(down && up);
        }

        [TestMethod]
        public void Keyhift()
        {
            bool down = false, up = false;
            Hook.I().AddKeyHook((Key k) =>
            {
                if (k.ToString().Contains("Shift"))
                {
                    down = true;
                }

                Console.WriteLine("d {0}", k.ToString());
            }, (Key k) =>
            {
                if (k.ToString().Contains("Shift"))
                {
                    up = true;
                }

                Console.WriteLine("u {0}", k.ToString());
            });

            Thread.Sleep(w);
            ks.KeyDown(VirtualKeyCode.SHIFT);
            ks.KeyUp(VirtualKeyCode.SHIFT);
            Assert.IsTrue(down);
            Assert.IsTrue(up);
        }

        [TestMethod]
        public void keyCmdM()
        {
            bool ctrl = false, m = false;
            Hook.I().AddKeyHook((Key k) =>
            {
                if (k == Key.M)
                {
                    m = true;
                }

                if (k.ToString().Contains("Control"))
                {
                    ctrl = true;
                }
            }, (Key k) => { });

            Thread.Sleep(w);
            ks.KeyDown(VirtualKeyCode.CONTROL);
            ks.KeyPress(VirtualKeyCode.VK_M);
            ks.KeyUp(VirtualKeyCode.CONTROL);
            Assert.IsTrue(ctrl && m);
        }

        [TestCleanup]
        public void cleanup()
        {
            Hook.Dispose();
        }
    }
}
