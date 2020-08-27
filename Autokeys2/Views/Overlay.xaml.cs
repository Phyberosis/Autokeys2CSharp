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
using Data;
using Events;
using InputHook;
using Monitors;

namespace Autokeys2.Views
{
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>

    public partial class Overlay : Window
    {
        private readonly Color RED = Colors.Red;
        private readonly Color GREEN;
        private readonly Color BLUE;

        public delegate void OnMouseLocationSelected(int x, int y, bool canceled = false);
        private double W, H;

        private bool shown = false;

        #region Window styles - Tool Window
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


            BLUE = (Color)FindResource("blue0");
            GREEN = (Color)FindResource("green0");

            //todo multimonitors?
            W = SystemParameters.PrimaryScreenWidth;
            H = SystemParameters.PrimaryScreenHeight;

            EventsBuiltin.RegisterListener<OnMouseLocationSelected>(EventID.SELECT_MOUSE_LOCATION, selectMouseLocation);
            EventsBuiltin.RegisterListener<MyColors>(EventID.BORDER_SHOW, show);
            EventsBuiltin.RegisterListener(EventID.BORDER_HIDE, (o)=> { hide(); });
        }

        private void show(MyColors c)
        {
            Color col;
            switch (c)
            {
                case MyColors.RED:
                    col = Colors.Red;
                    break;
                case MyColors.GREEN:
                    col = GREEN;
                    break;
                case MyColors.BLUE:
                    col = BLUE;
                    break;
                default:
                    col = Colors.White;
                    break;
            }

            lock (this) shown = true;
            Dispatcher.Invoke(() =>
            {
                border.BorderBrush = new SolidColorBrush(col);
                WindowState = WindowState.Maximized;
                Show();

                Action<Task> loop = null;
                loop = (t) =>
                {
                    bool cont = false;
                    lock (this) cont = shown;
                    if (!cont) return;
                    Dispatcher.Invoke(() => { Topmost = false; Topmost = true; });
                    Task.Delay(100).ContinueWith(loop);
                };
                loop(null);
            });
        }

        private void hide()
        {
            lock (this) shown = false;
            Dispatcher.Invoke(() =>
            {
                WindowState = WindowState.Minimized;
                Visibility = Visibility.Hidden;
                canvas.Children.Clear();
            });
        }

        private void selectMouseLocation(OnMouseLocationSelected callback)
        {
            Dispatcher.Invoke(() => { selectDelegate(callback); });
        }

        private void selectDelegate(OnMouseLocationSelected callback)
        {
            show(MyColors.BLUE);

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

                hide();
                
                System.Drawing.Point p = CursorMonitor.Position();
                callback(p.X, p.Y);
                Task.Delay(0).ContinueWith((t) =>
                {
                    Hook.I().RemoveMouseHook(mouseDown);
                });
            };
            Hook.I().AddMouseHook(mouseDown);

            Hook.OnKeyDelegate keyU = (k) => { };
            Hook.OnKeyDelegate keyD = null;
            keyD = (k) =>
            {
                if (k != Key.Escape)
                    return;

                Dispatcher.Invoke(() =>
                {
                    WindowState = WindowState.Minimized;
                    Visibility = Visibility.Hidden;
                });

                callback(0, 0, true);

                Task.Delay(0).ContinueWith((t) =>
                {
                    Hook.I().RemoveKeyHook(keyD, keyU);
                });
            };
            Hook.I().AddKeyHook(keyD, keyU);

            Action loop = null;
            loop = () =>
            {
                Topmost = true;

                canvas.Children.Clear();
                System.Drawing.Point p = CursorMonitor.Position();

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
        }
    }
}
