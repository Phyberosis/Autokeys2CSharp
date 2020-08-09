using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace OutputSimulator
{
    public class VirtualMouse
    {

        [DllImport("user32")]
        private static extern int SetCursorPos(int x, int y);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; /* right button up */

        public void Move(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void Move(Point p)
        {
            Move(p.X, p.Y);
        }

        public void LDown(int x, int y)
        {
            cmd(MOUSEEVENTF_LEFTDOWN, x, y);
        }

        public void LUp(int x, int y)
        {
            cmd(MOUSEEVENTF_LEFTUP, x, y);
        }

        public void RDown(int x, int y)
        {
            cmd(MOUSEEVENTF_RIGHTDOWN, x, y);
        }

        public void RUp(int x, int y)
        {
            cmd(MOUSEEVENTF_RIGHTUP, x, y);
        }

        private void cmd(int a, int x, int y)
        {
            SetCursorPos(x, y);
            mouse_event(a, 0, 0, 0, 0);
        }
    }
}
