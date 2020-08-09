using InputHook;
using Recordings;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;

namespace Autokeys2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RecordingManager recManager;
        private bool isRecording = false;

        private InputHandler inputHandler;

        public MainWindow()
        {
            InitializeComponent();
            recManager = new RecordingManager();
            inputHandler = new InputHandler();

            vewLeft.Visibility = Visibility.Collapsed;
            //Task.Delay(0).ContinueWith((t) =>
            //{
            //    WindowsInput.IKeyboardSimulator ks = new WindowsInput.KeyboardSimulator(new WindowsInput.InputSimulator());

            //    while(true)
            //    {
            //        ks.KeyDown(WindowsInput.Native.VirtualKeyCode.VK_X);
            //        Thread.Sleep(1000);
            //    };
            //});
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void showHideLeft(object sender, RoutedEventArgs e)
        {
            //if (vewLeft.IsVisible)
            //{
            //    vewLeft.Visibility = Visibility.Collapsed;
            //    btnExL.Content = "<";
            //}
            //else
            //{
            //    vewLeft.Visibility = Visibility.Visible;
            //    btnExL.Content = ">";
            //}
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
                recManager.RecordBegin();
                Action reset = () =>
                {
                    recManager.RecordEnd();
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
            recManager.Play();
            WindowState = WindowState.Minimized;
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
