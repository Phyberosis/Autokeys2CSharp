using Data;
using Events;
using Recordings;
using SourceChord.FluentWPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private Key proposedKey;

        private EventHandle<Overlay.OnMouseLocationSelected> selectMouseLocationHandle;

        public InfoTray(OpenLinkedListNode<Recording.Keyframe> node,
            KeyframeModel prev, RecordingModel.FocusContainer focusedTray)
        {
            InitializeComponent();
            BLUE = (Color)FindResource("blue0");
            trayFocusContainer = focusedTray;

            Model = new KeyframeModel(node, this);
            Model.Prev = prev;
            if(prev!=null)prev.Next = Model;
            this.DataContext = Model;

            lostFocusChild = () => { };

            selectMouseLocationHandle = EventsBuiltin.RegisterEvent<Overlay.OnMouseLocationSelected>(EventID.SELECT_MOUSE_LOCATION);
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

        private void focusChangedTray()
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
            if(focusChangedChild(txtTime)) lostFocusChild = () => 
            {
                long prev = Model.DataNode.Value.GetTime();
                float rawNow = -1;
                if (!float.TryParse(txtTime.Text, out rawNow) && rawNow >= 0)
                {
                    Model.SetTime(prev);
                }
                else
                {
                    Model.SetTime((long)Math.Round(rawNow * 1000));
                }
            };

            focusChangedTray();
        }

        private void txtInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(Model.HandlesKeys())
            {
                txtInfo.Text = "<new key>";
                proposedKey = Key.None;
                txtInfo.Foreground = new SolidColorBrush(Colors.Orange);
                Model.SaveState();

                e.Handled = true;
                if (focusChangedChild(txtInfo)) setFocusChild(txtInfo, brdInfo,
                    () =>
                    {
                        if (proposedKey == Key.None) Model.LoadState();
                        else Model.UpdateInfo(proposedKey);
                    });
            }
            else
            {
                bool forward = e.LeftButton == MouseButtonState.Pressed;
                Model.CycleMouseInfo(forward);
            }

            Focus();
            focusChangedTray();
            e.Handled = true;
        }

        private void txtDescription_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Model.HandlesKeys())
            {
                e.Handled = true;
                Model.UpdateDesc();
                //if (focusChangedChild(txtDescription)) setFocusChild(txtDescription, brdDesc, ()=>{ });
            }
            else
            {
                brdDesc.BorderBrush = new SolidColorBrush(Colors.Red);
                txtDescription.Foreground = new SolidColorBrush(Colors.Orange);
                string old = txtDescription.Text;
                txtDescription.Text = "<select new mouse position>";
                Overlay.OnMouseLocationSelected callback = (x, y, c) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        brdDesc.BorderBrush = new SolidColorBrush(Colors.Transparent);
                        txtDescription.Foreground = new SolidColorBrush(Colors.White);
                        if(c)
                        {
                            txtDescription.Text = old;
                        }
                        else
                        {
                            Model.UpdateDesc(x, y);
                        }
                    });
                };
                selectMouseLocationHandle.Notify(callback);
            }

            Focus();
            focusChangedTray();
            e.Handled = true;
        }

        private void traySelect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (focusChangedChild(traySelect)) lostFocusChild = () =>
            { };

            this.Focus();
            focusChangedTray();
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

        private void txtInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Model.HandlesKeys()) return;

            txtInfo.Foreground = new SolidColorBrush(BLUE);
            Model.Info = e.Key.ToString();
            proposedKey = e.Key;
        }

        private void txtTime_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                focusChangedChild(null);
                Focus();
            }
        }

        //public InfoTray(string info, string description)
        //{
        //    InitializeComponent();
        //}
    }

    public class KeyframeModel
    {
        private string info, desc, time;
        private struct State
        {
            public string Info, Desc, Time;
            public State(KeyframeModel state)
            {
                Info = state.info;
                Desc = state.desc;
                Time = state.time;
            }
        }
        private State memory;

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
                time = value;
                if (ui != null) ui.Dispatcher.Invoke(() => ui.txtTime.Text = time);
            }
        }

        public KeyframeModel Next, Prev;
        public OpenLinkedListNode<Recording.Keyframe> DataNode;
        private InfoTray ui;

        private MouseAction[] actions;
        private int currentAction;

        public KeyframeModel(OpenLinkedListNode<Recording.Keyframe> node,
            InfoTray ui)
        {
            DataNode = node;
            Info = node.Value.GetInfo();
            Description = node.Value.GetDescription();
            Time = convertTime(node.Value.GetTime());

            actions = (MouseAction[])Enum.GetValues(typeof(MouseAction));
            var ca = node.Value.GetMouseAction();
            for(int i = 0; i < actions.Length; i++)
            {
                if(actions[i] == ca)
                {
                    currentAction = i;
                    break;
                }
            }

            this.ui = ui;
        }

        private string convertTime(long t)
        {
            return ((float)t / 1000).ToString();
        }

        public void SlideTime(long delta)
        {
            DataNode.Value.SlideTime(delta);
            Time = convertTime(DataNode.Value.GetTime());
        }

        public void SetTime(long now)
        {
            now = now < 0 ? 0 : now;
            if(Prev != null)
            {
                var pt = Prev.DataNode.Value.GetTime();
                if (now < pt)
                    now = pt;
            }

            long diff = now - DataNode.Value.GetTime();
            var curr = Next;
            while (curr != null)
            {
                curr.SlideTime(diff);
                //Console.WriteLine(curr.DataNode.Value.GetTime());
                curr = curr.Next;
            }

            DataNode.Value.SetTime(now);
            Time = convertTime(now);
        }

        public void SaveState()
        {
            memory = new State(this);
        }

        public void LoadState()
        {
            Time = memory.Time;
            Info = memory.Info;
            Description = memory.Desc;
        }

        public void UpdateInfo(Key k)
        {
            var kf = DataNode.Value;
            var newKF = new Recording.KeyFrameK(kf.GetKeyAction(), k, kf.GetTime());
            DataNode.Value = newKF;
        }

        public void CycleMouseInfo(bool forward)
        {
            var kf = DataNode.Value;
            currentAction = forward? currentAction+1 : currentAction-1;
            int l = Enum.GetNames(typeof(MouseAction)).Length;
            if(forward && currentAction >= l)
            {
                currentAction = 1;
            }
            else if (!forward && currentAction <= 0)
            {
                currentAction = l-1;
            }
            int[] loc = kf.GetLocation();
            var newKF = new Recording.KeyFrameM(actions[currentAction], loc[0], loc[1], kf.GetTime());
            DataNode.Value = newKF;

            Info = newKF.GetInfo();
        }

        public void UpdateDesc(int x, int y)
        {
            var kf = DataNode.Value;
            var newKF = new Recording.KeyFrameM(actions[currentAction], x, y, kf.GetTime());
            DataNode.Value = newKF;

            Description = newKF.GetDescription();
        }

        public void UpdateDesc()
        {
            var kf = DataNode.Value;
            var wpfKey = KeyInterop.KeyFromVirtualKey((int)kf.GetKey());
            var ka = kf.GetKeyAction();
            ka++;
            if ((int)ka >= Enum.GetNames(typeof(KeyActions)).Length) ka = (KeyActions)1;
            var newKF = new Recording.KeyFrameK(ka, kf.GetKey(), kf.GetTime());
            DataNode.Value = newKF;

            Description = newKF.GetDescription();
        }

        public bool HandlesKeys() { return DataNode.Value.HandlesKeys(); }
    }
}
