using Events;
using Recordings;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Autokeys2.Views
{
    /// <summary>
    /// Interaction logic for InfoTray.xaml
    /// </summary>

    public partial class InfoTray : UserControl
    {
        private readonly Color BLUE;

        public readonly KeyframeModel Model;

        private Action lostFocusChild;
        private object focused = null;
        private bool trayFocused = false;
        private RecordingModel.FocusContainer trayFocusContainer;

        protected Recording.Keyframe keyframe;

        public static InfoTray BuildNew(Recording.Keyframe kf, RecordingModel.FocusContainer f)
        {
            InfoTray tray;
            if (kf.Type == Recording.KFType.KEY)
            {
                tray = new InfoTrayK(kf, f);
            }
            else
            {
                tray = new InfoTrayM(kf, f);
            }

            return tray;
        }

        protected InfoTray(Recording.Keyframe kf, RecordingModel.FocusContainer focusedTray)
        {
            InitializeComponent();

            keyframe = kf;
            BLUE = (Color)FindResource("blue0");
            trayFocusContainer = focusedTray;

            //Model = new KeyframeModel(convertTime(kf.GetTime());
            Model = new KeyframeModel(this);
            Model.Time = convertTime(kf.GetTime());
            kf.OnUpdateTime((t) => { Model.Time = convertTime(t); });

            this.DataContext = Model;

            lostFocusChild = () => { };

            //Task.Delay(0).ContinueWith((t) =>
            //{
            //    int i = 0;
            //    while(true)
            //    {
            //        Model.Time = i.ToString();
            //        i++;
            //        Thread.Sleep(500);
            //    }
            //});
        }

        private static string convertTime(long t)
        {
            float i = (float)t / 1000f;
            string s = i.ToString();
            if (i * 10 % 10 == 0) s += ".0";
            return s;
        }

        public Recording.Keyframe GetKeyframe()
        {
            return keyframe;
        }

        public void FocusChangedTray()
        {
            if (trayFocused) return;
            trayFocused = true;

            var f = trayFocusContainer;
            f.FocusChanged();
            f.Focused = this;

            traySelect.Background = new SolidColorBrush(BLUE);
            brdAll.BorderBrush = new SolidColorBrush(BLUE);
            //Console.WriteLine("0");
            f.FocusChanged = () =>
            {
                //Console.WriteLine("2");
                focusChangedChild(null);
                lostFocusChild = () => { };
                brdAll.BorderBrush = new SolidColorBrush(Colors.Transparent);
                traySelect.Background = new SolidColorBrush(Colors.LightGray);
                trayFocused = false;
            };
            //Console.WriteLine("1");
        }

        private bool focusChangedChild(object txt)
        {
            if (focused != null && focused.Equals(txt)) return false;

            lostFocusChild();
            focused = txt;

            return true;
        }

        //sets highlight border and reset callback
        private void setFocusChild(TextBlock t, Border b, Action callback)
        {
            t.Focus();

            b.BorderBrush = new SolidColorBrush(Colors.Red);
            lostFocusChild = () =>
            {
                b.BorderBrush = new SolidColorBrush(Colors.Transparent);
                t.Foreground = new SolidColorBrush(Colors.White);
                callback();
            };
        }

        private void txtTime_GotFocus(object sender, RoutedEventArgs e)
        {
            txtTime.SelectAll();
            string prev = txtTime.Text;
            if(focusChangedChild(txtTime)) lostFocusChild = () => 
            {
                float rawNow = -1;
                if (float.TryParse(txtTime.Text, out rawNow))
                {
                    keyframe.SetTime(rawNow);
                }
                else
                {
                    txtTime.Text = prev;
                }
            };

            FocusChangedTray();
        }

        public virtual void txtInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            FocusChangedTray();
            e.Handled = true;
        }

        public virtual void txtDescription_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Focus();
            FocusChangedTray();
            e.Handled = true;
        }

        private void traySelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (focusChangedChild(traySelect)) lostFocusChild = () =>
            { };

            this.Focus();
            FocusChangedTray();
            e.Handled = true;
        }

        private void txtInfo_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (focused != null && focused.Equals(txtInfo)) return;

            Color c = ((bool)e.NewValue) ? BLUE : Colors.Transparent;
            brdInfo.BorderBrush = new SolidColorBrush(c);
        }

        private void txtDescription_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (focused != null && focused.Equals(txtDescription)) return;

            Color c = ((bool)e.NewValue) ? BLUE : Colors.Transparent;
            brdDesc.BorderBrush = new SolidColorBrush(c);
        }

        private void traySelect_IsMouseDirectlyOverChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Color c = ((bool)e.NewValue) ? BLUE : Colors.Transparent;
            traySelect.BorderBrush = new SolidColorBrush(c);
        }

        public virtual void txtInfo_KeyDown(object sender, KeyEventArgs e)
        { }

        private void txtTime_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                focusChangedChild(null);
                Focus();
            }
            if(e.Key == Key.Escape)
            {
                focusChangedChild(null);
                Focus();
                txtTime.Text = Model.Time;
            }
        }

        private class InfoTrayM : InfoTray
        {
            private Recording.KeyframeM kfM;
            private EventHandle<Overlay.Callbacks> selectMouseLocationHandle;

            public InfoTrayM(Recording.Keyframe kf, RecordingModel.FocusContainer f) : base(kf, f)
            {
                kfM = (Recording.KeyframeM)kf;
                selectMouseLocationHandle = EventsBuiltin.RegisterEvent<Overlay.Callbacks>(EventID.SELECT_MOUSE_LOCATION);
                kfM.OnUpdateLoc((x, y) => { Model.Description = convertDesc(x, y); });
                kfM.OnUpdateMA((ma) => { Model.Info = convertInfo(ma); });

                int[] loc = kfM.GetLoc();
                Model.Description = convertDesc(loc[0], loc[1]);
                Model.Info = convertInfo(kfM.GetMA());
            }

            public string convertDesc(int x, int y)
            {
                return "at (" + x + ", " + y + ")";
            }

            private string convertDescUpdate(int x, int y)
            {
                return "< changing to (" + x + ", " + y + ") >";
            }

            public string convertInfo(MouseAction ma)
            {
                return MouseActionVerbalizer.Convert(ma) + MouseActionVerbalizer.GetType(ma);
            }

            public override void txtDescription_MouseDown(object sender, MouseButtonEventArgs e)
            {
                brdDesc.BorderBrush = new SolidColorBrush(Colors.Red);
                txtDescription.Foreground = new SolidColorBrush(Colors.Orange);
                string old = txtDescription.Text;
                //txtDescription.Text = "<select new mouse position>";
                Overlay.OnMouseLocationSelected callback = (x, y, c) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        brdDesc.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        txtDescription.Foreground = new SolidColorBrush(Colors.White);
                        if (c)
                        {
                            txtDescription.Text = old;
                        }
                        else
                        {
                            kfM.UpdateLocation(x, y);
                        }
                    });
                };
                Action<int, int> update = (x, y) =>
                {
                    Dispatcher.Invoke(() => { txtDescription.Text = convertDescUpdate(x, y); });
                };
                selectMouseLocationHandle.Notify(new Overlay.Callbacks(callback, update));

                base.txtDescription_MouseDown(sender, e);
            }

            public override void txtInfo_MouseDown(object sender, MouseButtonEventArgs e)
            {
                bool forward = e.LeftButton == MouseButtonState.Pressed;
                kfM.CycleMouseActions(forward);

                base.txtInfo_MouseDown(sender, e);
            }

        }

        private class InfoTrayK : InfoTray
        {
            private Recording.KeyframeK kfK;
            private Key proposedKey;

            public InfoTrayK(Recording.Keyframe kf, RecordingModel.FocusContainer f) : base(kf, f)
            {
                kfK = (Recording.KeyframeK)kf;
                kfK.OnUpdateKA((ka) => { Model.Description = convertDesc(ka); });
                kfK.OnUpdateKey((k) => { Model.Info = convertInfo(k); });

                Model.Info = convertInfo(kfK.GetKey());
                Model.Description = convertDesc(kfK.GetKA());
            }

            public string convertDesc(KeyActions ka)
            {
                return ka.ToString();
            }

            public string convertInfo(Key k)
            {
                return k.ToString();
            }

            public override void txtDescription_MouseDown(object sender, MouseButtonEventArgs e)
            {
                kfK.CycleKeyActions();

                base.txtDescription_MouseDown(sender, e);
            }

            public override void txtInfo_MouseDown(object sender, MouseButtonEventArgs e)
            {
                var prev = txtInfo.Text;
                txtInfo.Text = "<new key>";
                txtInfo.Foreground = new SolidColorBrush(Colors.Orange);

                e.Handled = true;
                if (focusChangedChild(txtInfo)) setFocusChild(txtInfo, brdInfo,
                    () =>
                    {
                        if (proposedKey == Key.None) txtInfo.Text = prev;
                        else kfK.SetKey(proposedKey);
                    });

                base.txtInfo_MouseDown(sender, e);
            }

            public override void txtInfo_KeyDown(object sender, KeyEventArgs e)
            {
                txtInfo.Foreground = new SolidColorBrush(BLUE);
                Model.Info = e.Key.ToString();
                proposedKey = e.Key;
            }
        }
    }

    public class KeyframeModel
    {
        private string info, desc, time;
        //private struct State
        //{
        //    public string Info, Desc, Time;
        //    public State(KeyframeModel state)
        //    {
        //        Info = state.info;
        //        Desc = state.desc;
        //        Time = state.time;
        //    }
        //}
        //private State memory;

        public string Info
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
                if (ui != null) ui.Dispatcher.Invoke(() => ui.txtInfo.Text = info);
            }
        }
        public string Description 
        {
            get
            {
                return desc;
            }
            set
            {
                desc = value;
                if (ui != null) ui.Dispatcher.Invoke(() => ui.txtDescription.Text = desc);
            }
        }
        public string Time 
        { 
            get
            {
                return time;
            }
            set
            {
                //Console.WriteLine(">>>>>>>>" +value);
                time = value;
                if (ui != null) ui.Dispatcher.Invoke(() => ui.txtTime.Text = time);
            }
        }

        public KeyframeModel Next, Prev;
        private InfoTray ui;

        //private MouseAction[] actions;
        //private int currentAction;

        public KeyframeModel(InfoTray ui)
        {
            this.ui = ui;
            //Console.WriteLine("@@@@@@@@@@@@@@@@@"+ui);
            //actions = (MouseAction[])Enum.GetValues(typeof(MouseAction));
            //var ca = data.GetMouseAction();
            //for(int i = 0; i < actions.Length; i++)
            //{
            //    if(actions[i] == ca)
            //    {
            //        currentAction = i;
            //        break;
            //    }
            //}
        }
    }
}
