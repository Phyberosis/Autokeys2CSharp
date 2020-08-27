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
using Data;

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

        public static SettingsModel SettingsModel;

        public MainWindow()
        {
            InitializeComponent();
            SettingsModel = new SettingsModel(this);
            txtRepeats.DataContext = SettingsModel;
            txtSpeed.DataContext = SettingsModel;

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
            recManager.Play(SettingsModel.GetSpeed(), SettingsModel.GetRepeats());
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
            handleTextboxShortcuts(e, SettingsModel.RevertRepeats);
        }

        private void txtSpeed_KeyDown(object sender, KeyEventArgs e)
        {
            handleTextboxShortcuts(e, SettingsModel.RevertSpeed);
        }
    }

    public class SettingsModel
    {
        private Settings current, last;

        private MainWindow ui;

        public string Repeats
        {
            get
            {
                return current.Repeats.ToString();
            }
            set
            {
                int r;
                if(int.TryParse(value, out r))
                {
                    last.Repeats = current.Repeats;
                    current.Repeats = r;
                }
                ui.txtRepeats.Text = Repeats;
            }
        }

        public string Speed
        {
            get
            {
                string s = current.Speed.ToString();
                if (Math.Round(current.Speed) == current.Speed) s += ".0";
                return s;
            }
            set
            {
                float s;
                if(float.TryParse(value, out s))
                {
                    last.Speed = current.Speed;
                    current.Speed = s;
                }

                ui.txtSpeed.Text = Speed;
            }
        }

        public SettingsModel(MainWindow ui)
        {
            current = new Settings();
            last = new Settings();
            current.Repeats = 0;
            current.Speed = 1;
            this.ui = ui;
        }

        public Settings GetSettings()
        {
            return current;
        }

        public void LoadSettings(Settings s)
        {
            current = s;
            Speed = current.Speed.ToString();
            Repeats = current.Repeats.ToString();
        }

        public int GetRepeats()
        {
            return current.Repeats;
        }

        public float GetSpeed()
        {
            return current.Speed;
        }

        public void RevertRepeats()
        {
            Repeats = last.Repeats.ToString();
        }

        public void RevertSpeed()
        {
            Speed = last.Speed.ToString();
        }
    }

}
