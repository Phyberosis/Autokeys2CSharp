﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InputHook
{
    public enum MouseAction
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    public class Hook
    {
        private delegate IntPtr LowLevelProc(int nCode, IntPtr wParam, IntPtr lParam);

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_KEYUP = 0x0101;

        // keyboard
        private LowLevelProc _kproc;
        private static IntPtr _khookID = IntPtr.Zero;
        public delegate void OnKeyDelegate(Keys k);
        private LinkedList<OnKeyDelegate> onKeyUp;
        private LinkedList<OnKeyDelegate> onKeyDn;

        // mouse
        private LowLevelProc _mproc;
        private static IntPtr _mhookID = IntPtr.Zero;
        public delegate void OnMouseDelegate(MouseAction a, int x, int y);
        private LinkedList<OnMouseDelegate> onMouse;


        // singleton
        private static Hook inst = null;
        public static Hook I()
        {
            if (inst == null)
                inst = new Hook();

            return inst;
        }

        private Hook()
        {
            _kproc = kHookReception;
            _mproc = mHookReception;

            _khookID = setkHook(_kproc);
            _mhookID = SetmHook(_mproc);

            onKeyUp = new LinkedList<OnKeyDelegate>();
            onKeyDn = new LinkedList<OnKeyDelegate>();
            onMouse = new LinkedList<OnMouseDelegate>();
        }

        public void AddKeyHook(OnKeyDelegate onDown, OnKeyDelegate onUp)
        {
            Task.Delay(0).ContinueWith((t) => {
                lock (this)
                {
                    onKeyDn.AddLast(onDown);
                    onKeyUp.AddLast(onUp);
                }
            });
        }

        public void RemoveKeyHook(OnKeyDelegate onDown, OnKeyDelegate onUp)
        {
            Task.Delay(0).ContinueWith((t) => {
                lock (this)
                {
                    onKeyDn.Remove(onDown);
                    onKeyUp.Remove(onUp);
                }
            });
        }

        public void AddMouseHook(OnMouseDelegate onMouse)
        {
            Task.Delay(0).ContinueWith((t) =>
            {
                lock (this)
                    this.onMouse.AddLast(onMouse);
            });
        }

        public void RemoveMouseHook(OnMouseDelegate onMouse)
        {
            Task.Delay(0).ContinueWith((t) =>
            {
                lock (this)
                    this.onMouse.Remove(onMouse);
            });
        }

        public static void Dispose()
        {
            inst?.dispose();
        }

        private void dispose()
        {
            UnhookWindowsHookEx(_khookID);
            UnhookWindowsHookEx(_mhookID);
        }

        // keystroke interceptor
        private static IntPtr setkHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr kHookReception(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Task.Delay(0).ContinueWith((t) =>
            {
                lock (this)
                {
                    if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        foreach (var fn in onKeyDn)
                        {
                            fn((Keys)vkCode);
                        }
                        //Console.WriteLine((Keys)vkCode);
                        //MessageBox.Show(((Keys)vkCode).ToString());
                    }
                    else if (nCode >= 0 && wParam == (IntPtr)WM_KEYUP)
                    {
                        int vkCode = Marshal.ReadInt32(lParam);
                        foreach (var fn in onKeyUp)
                        {
                            fn((Keys)vkCode);
                        }
                    }
                }
            });
            return CallNextHookEx(_khookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private const int WH_MOUSE_LL = 14;
        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private static IntPtr SetmHook(LowLevelProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr mHookReception(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Task.Delay(0).ContinueWith((t) =>
            {
                lock (this)
                {
                    if (nCode >= 0)
                    //&& MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
                    {
                        MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                        foreach (var fn in onMouse)
                        {
                            fn((MouseAction)wParam, hookStruct.pt.x, hookStruct.pt.y);
                        }
                        //Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
                    }
                }
            });
            return CallNextHookEx(_mhookID, nCode, wParam, lParam);
        }
    }
}
