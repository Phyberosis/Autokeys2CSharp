using InputHook;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;

namespace Autokeys2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Recording rec;
        private bool isRecording = false;

        private InputHandler inputHandler;

        public MainWindow()
        {
            InitializeComponent();
            rec = new Recording();
            inputHandler = new InputHandler();
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnRecord_Click(object sender, RoutedEventArgs e)
        {
            lock(this)
            {
                if (isRecording) return;
                isRecording = true;

                btnRecord.Visibility = Visibility.Hidden;
                taskbarInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
                WindowState = WindowState.Minimized;
                rec.Begin();
                Action reset = () =>
                {
                    rec.End();
                    taskbarInfo.ProgressState = TaskbarItemProgressState.None;
                    btnRecord.Visibility = Visibility.Visible;
                    isRecording = false;

                    WindowState = WindowState.Normal;
                    this.Activate();

                    this.Topmost = false;
                    this.Topmost = true;
                };
                inputHandler.WaitForStopRec(() => { if (isRecording) this.Dispatcher.BeginInvoke(reset); });
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnMin_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hook.Dispose();
        }
    }
}
