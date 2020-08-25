using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Events;
using InputHook;

namespace Autokeys2.Views
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    
    public partial class Overlay : Window
    {
        public delegate void OnMouseLocationSelected(int x, int y);
        private double W, H;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        public static Point GetMousePosition()
        {
            Win32Point w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        #region Window styles
        //https://social.msdn.microsoft.com/Forums/en-US/9c4ada92-5065-4abb-a295-d62e5ddaf2b1/wpf-window-is-showen-in-alttab-list-though-windowstylequotnonequot-amp?forum=wpf

        [Flags]
        public enum ExtendedWindowStyles
        {
            // ...
            WS_EX_TOOLWINDOW = 0x00000080,
            // ...
        }

        public enum GetWindowLongFields
        {
            // ...
            GWL_EXSTYLE = (-20),
            // ...
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        public static IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong)
        {
            int error = 0;
            IntPtr result = IntPtr.Zero;
            // Win32 SetWindowLong doesn't clear error on success
            SetLastError(0);

            if (IntPtr.Size == 4)
            {
                // use SetWindowLong
                Int32 tempResult = IntSetWindowLong(hWnd, nIndex, IntPtrToInt32(dwNewLong));
                error = Marshal.GetLastWin32Error();
                result = new IntPtr(tempResult);
            }
            else
            {
                // use SetWindowLongPtr
                result = IntSetWindowLongPtr(hWnd, nIndex, dwNewLong);
                error = Marshal.GetLastWin32Error();
            }

            if ((result == IntPtr.Zero) && (error != 0))
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return result;
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        private static extern IntPtr IntSetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        private static extern Int32 IntSetWindowLong(IntPtr hWnd, int nIndex, Int32 dwNewLong);

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("kernel32.dll", EntryPoint = "SetLastError")]
        public static extern void SetLastError(int dwErrorCode);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

        }
        #endregion

        public Overlay()
        {
            InitializeComponent();

            //todo multimonitors?
            W = SystemParameters.PrimaryScreenWidth;
            H = SystemParameters.PrimaryScreenHeight;

            EventsBuiltin.RegisterListener<OnMouseLocationSelected>(EventID.SELECT_MOUSE_LOCATION, selectMouseLocation);
        }

        private void selectMouseLocation(OnMouseLocationSelected callback)
        {
            Dispatcher.Invoke(() => { selectDelegate(callback); });
        }

        private void selectDelegate(OnMouseLocationSelected callback)
        {
            WindowState = WindowState.Maximized;
            Show();

            object syncLock = new object();
            bool followMouse = true;
            Hook.OnMouseDelegate mouseDown = null;
            mouseDown = (ma, x, y) =>
            {
                if (ma != MouseAction.WM_LBUTTONDOWN) return;

                lock (syncLock)
                {
                    followMouse = false;
                }

                Dispatcher.Invoke(() =>
                {
                    WindowState = WindowState.Minimized;
                    Visibility = Visibility.Hidden;
                });

                Win32Point p = new Win32Point();
                GetCursorPos(ref p);
                callback(p.X, p.Y);
                Task.Delay(0).ContinueWith((t) =>
                {
                    Hook.I().RemoveMouseHook(mouseDown);
                });
            };
            Hook.I().AddMouseHook(mouseDown);


            Color BLUE = (Color)FindResource("blue0");

            Action loop = null;
            loop = () =>
            {
                Topmost = true;

                canvas.Children.Clear();
                Win32Point p = new Win32Point();
                GetCursorPos(ref p);

                Line h = new Line();
                h.Stroke = new SolidColorBrush(BLUE);
                h.StrokeThickness = 2;
                h.X1 = 0; h.X2 = W;
                h.Y1 = p.Y; h.Y2 = p.Y;

                Line v = new Line();
                v.Stroke = new SolidColorBrush(BLUE);
                v.StrokeThickness = 2;
                v.X1 = p.X; v.X2 = p.X;
                v.Y1 = 0; v.Y2 = H;

                canvas.Children.Add(h);
                canvas.Children.Add(v);

                //return;
                lock (syncLock)
                {
                    if (!followMouse) return;
                }

                Task.Delay(16).ContinueWith((t) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        loop();
                    });
                });

            };
            Dispatcher.Invoke(loop);

            //while (true)
            //{


            //    //canvas.Children.Clear();
            //    //Win32Point p = new Win32Point();
            //    //GetCursorPos(ref p);

            //    //Line h = new Line();
            //    //h.Stroke = new SolidColorBrush(BLUE);
            //    //h.StrokeThickness = 2;
            //    //h.X1 = 0; h.X2 = W;
            //    //h.Y1 = p.Y; h.Y2 = p.Y;

            //    //Line v = new Line();
            //    //v.Stroke = new SolidColorBrush(BLUE);
            //    //v.StrokeThickness = 2;
            //    //v.X1 = p.X; v.X2 = p.X;
            //    //v.Y1 = 0; v.Y2 = H;

            //    //canvas.Children.Add(h);
            //    //canvas.Children.Add(v);
            //}
        }
    }
}
