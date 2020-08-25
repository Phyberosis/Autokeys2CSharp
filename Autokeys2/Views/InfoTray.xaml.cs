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

        public readonly KeyFrameModel Model;

        private Action lostFocusChild;
        private object focused = null;
        private bool trayFocused = false;
        private RecordingModel.FocusContainer trayFocusContainer;

        private Key proposedKey;

        public InfoTray(OpenLinkedListNode<Recording.KeyFrame> node,
            KeyFrameModel prev, RecordingModel.FocusContainer focusedTray)
        {
            InitializeComponent();
            BLUE = (Color)FindResource("blue0");
            trayFocusContainer = focusedTray;

            Model = new KeyFrameModel(node, this);
            Model.Prev = prev;
            if(prev!=null)prev.Next = Model;
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

        private void focusChangedTray()
        {
            trayFocused = true;

            var f = trayFocusContainer;
            f.FocusChanged();

            f.Focused = this;
            brdAll.BorderBrush = new SolidColorBrush(BLUE);

            f.FocusChanged = () =>
            {
                focusChangedChild(null);
                lostFocusChild = () => { };
                brdAll.BorderBrush = new SolidColorBrush(Colors.Transparent);
                trayFocused = false;
            };
        }

        private bool focusChangedChild(object txt)
        {
            if (focused != null && focused.Equals(txt)) return false;

            if (!trayFocused) focusChangedTray();

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
                    return;
                }

                long now = (long)Math.Round(rawNow * 1000);
                long diff = now - prev;
                var curr = Model;
                while(curr != null)
                {
                    curr.SlideTime(diff);
                    curr = curr.Next;
                }
            };
        }

        private void txtInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtInfo.Text = "<new key>";
            proposedKey = Key.None;
            txtInfo.Foreground = new SolidColorBrush(Colors.Orange);
            Model.SaveState();

            e.Handled = true;
            if (focusChangedChild(txtInfo)) setFocusChild(txtInfo, brdInfo,
                ()=>
                {
                    if (proposedKey == Key.None) Model.LoadState();
                    else Model.UpdateInfo(proposedKey);
                });
        }

        private void txtDescription_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            Model.UpdateDesc();
            if (focusChangedChild(txtDescription)) setFocusChild(txtDescription, brdDesc, ()=>{ });
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

        private void txtInfo_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Model.HandlesKeys()) return;

            txtInfo.Foreground = new SolidColorBrush(BLUE);
            Model.Info = e.Key.ToString();
            proposedKey = e.Key;
        }

        //public InfoTray(string info, string description)
        //{
        //    InitializeComponent();
        //}
    }

    public class KeyFrameModel
    {
        private string info, desc, time;
        private struct State
        {
            public string Info, Desc, Time;
            public State(KeyFrameModel state)
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

        public KeyFrameModel Next, Prev;
        public OpenLinkedListNode<Recording.KeyFrame> DataNode;
        private InfoTray ui;

        //public event PropertyChangedEventHandler PropertyChanged;
        //private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //protected virtual void OnPropertyChanged(string propertyName)
        //{

        //    PropertyChangedEventHandler handler = this.PropertyChanged;
        //    if (handler != null)
        //    {
        //        var e = new PropertyChangedEventArgs(propertyName);
        //        handler(this, e);
        //    }
        //}

        public KeyFrameModel(OpenLinkedListNode<Recording.KeyFrame> node,
            InfoTray ui)
        {
            DataNode = node;
            Info = node.Value.GetInfo();
            Description = node.Value.GetDescription();
            Time = convertTime(node.Value.GetTime());

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

        public void SetTime(long t)
        {
            Time = convertTime(t);
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
            var formsKey = (System.Windows.Forms.Keys)KeyInterop.VirtualKeyFromKey(k);
            var newKF = new Recording.KeyFrameK(kf.GetKeyAction(), formsKey, kf.GetTime());
            DataNode.Value = newKF;
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
