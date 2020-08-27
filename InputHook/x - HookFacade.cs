//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Runtime.InteropServices;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;
//using System.Windows.Forms.VisualStyles;

//namespace InputHook
//{
//    public enum MouseAction
//    {
//        WM_LBUTTONDOWN = 0x0201,
//        WM_LBUTTONUP = 0x0202,
//        WM_MOUSEMOVE = 0x0200,
//        WM_MOUSEWHEEL = 0x020A,
//        WM_RBUTTONDOWN = 0x0204,
//        WM_RBUTTONUP = 0x0205
//    }

//    //public enum KeyAction
//    //{
//    //    WM_KEYDOWN = 0x0100,
//    //    WM_KEYUP = 0x0101
//    //}

//    public class Hook
//    {
//        private Original orig;
//        private GlobalKeyboardHook withAlt;
//        private static Hook instance = null;

//        public delegate void OnMouseDelegate(MouseAction a, int x, int y);
//        private OnMouseDelegate onMouse;

//        public delegate void OnKeyDelegate(Keys k);
//        private OnKeyDelegate onKeyUp;
//        private OnKeyDelegate onKeyDn;

//        public static Hook I()
//        {
//            if (instance == null)
//            {
//                instance = new Hook();
//            }

//            return instance;
//        }

//        private Hook()
//        {
//            orig = new Original();

//            orig.SetMouseHook((ma, x, y) =>
//            {
//                this.onMouse?.Invoke(ma, x, y);
//            });

//            withAlt = new GlobalKeyboardHook();
//            withAlt.KeyboardPressed += onKeyReception;
//        }

//        private void onKeyReception(object sender, GlobalKeyboardHookEventArgs e)
//        {
//            //Console.WriteLine("VC {0}, state {1}", ((Keys)e.KeyboardData.VirtualCode).ToString(), e.KeyboardState.ToString());

//            Keys k = ((Keys)e.KeyboardData.VirtualCode);
//            if e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown || e.KeyboardState == GlobalKeyboardHook.KeyboardState.SysKeyDown
//        }

//        public void SetKeyHook(OnKeyDelegate onDown, OnKeyDelegate onUp)
//        {
//            onKeyDn = onDown;
//            onKeyUp = onUp;
//        }

//        public void SetMouseHook(OnMouseDelegate onMouse)
//        {
//            this.onMouse = onMouse;
//        }

//        public void Unhook()
//        {
//            orig.Unhook();
//            withAlt.Dispose();
//        }
//    }
//}