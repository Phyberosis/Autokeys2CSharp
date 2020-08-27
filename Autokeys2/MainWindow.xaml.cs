using Autokeys2.Views;
using InputHook;
using Recordings;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shell;
using Monitors;
using System.Windows.Media;
using System.Windows.Controls;

namespace Autokeys2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private RecordingManager recManager;
        private bool isRecording = false;

        private Overlay overlay;

        private Settings settings;

        public MainWindow()
        {
            InitializeComponent();
            settings = new Settings(this);
            txtRepeats.DataContext = settings;
            txtSpeed.DataContext = settings;

            recManager = new RecordingManager();

            overlay = new Overlay();
            overlay.Show();

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
            brdAll.Focus();
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void showHideLeft(object sender, RoutedEventArgs e)
        {
            if (vewLeft.IsVisible)
            {
                vewLeft.Visibility = Visibility.Collapsed;
                btnExL.Content = "<";
            }
            else
            {
                vewLeft.Visibility = Visibility.Visible;
                btnExL.Content = ">";
            }
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
                GestureMonitor.WaitForStopRec(() => { if (isRecording) this.Dispatcher.BeginInvoke(reset); });
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            recManager.Play(settings.GetSpeed(), settings.GetRepeats());
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
            overlay.Close();
            Hook.Dispose();
        }

        private void handleTextboxShortcuts(KeyEventArgs e, Action onCancel)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    brdAll.Focus();
                    break;
                case Key.Escape:
                    brdAll.Focus();
                    onCancel();
                    break;
                default:
                    break;
            }
        }

        private void txtRepeats_KeyDown(object sender, KeyEventArgs e)
        {
            handleTextboxShortcuts(e, settings.RevertRepeats);
        }

        private void txtSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            handleTextboxShortcuts(e, settings.RevertSpeed);
        }
    }

    public class Settings
    {
        private const float MAX_SPEED = 20;
        private const float MIN_SPEED = 0.25f;

        private int repeats, lastR;
        private float speed, lastS;

        private MainWindow ui;

        public string Repeats
        {
            get
            {
                return repeats.ToString();
            }
            set
            {
                if(value.Length > 3)
                {
                    value = "999";
                }
                int r;
                if(int.TryParse(value, out r))
                {
                    if (r < 0) r = 0;
                    lastR = repeats;
                    repeats = r;
                }
                ui.txtRepeats.Text = Repeats;
            }
        }

        public string Speed
        {
            get
            {
                string s = speed.ToString();
                if (Math.Round(speed) == speed) s += ".0";
                return s;
            }
            set
            {
                float s;
                if(float.TryParse(value, out s))
                {
                    if (s < MIN_SPEED) s = MIN_SPEED;
                    if (s > MAX_SPEED) s = MAX_SPEED;
                    lastS = speed;
                    speed = s;
                }

                ui.txtSpeed.Text = Speed;
            }
        }

        public Settings(MainWindow ui)
        {
            repeats = 0;
            speed = 1;
            this.ui = ui;
        }

        public int GetRepeats()
        {
            return repeats;
        }

        public float GetSpeed()
        {
            return speed;
        }

        public void RevertRepeats()
        {
            Repeats = lastR.ToString();
        }

        public void RevertSpeed()
        {
            Speed = lastS.ToString();
        }
    }

}
