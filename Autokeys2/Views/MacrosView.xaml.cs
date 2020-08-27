using Autokeys2.Views.Trays;
using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Events;
using Recordings;
using System.Collections.Generic;
using Data;
using MaterialDesignThemes.Wpf;
using System.Windows.Markup;
using System.IO;
using System.CodeDom;
using System.Linq;
using System.Windows.Shell;

namespace Autokeys2.Views
{
    public partial class MacrosView : UserControl
    {
        private Recording data;
        private RecordingModel model;
        public MacrosView()
        {
            InitializeComponent();

            EventsBuiltin.RegisterListener<Recording>(EventID.REC, 
                (r)=>
                {
                    this.Dispatcher.Invoke(()=> { onRec(r); });
                });

            //test
            //model = new RecordingModel();
            //keyframesControl.DataContext = model;
            //var n1 = new OpenLinkedListNode<Recording.KeyFrame>(new Recording.KeyFrameK(KeyActions.PRESS, System.Windows.Forms.Keys.A, 1200));
            //var n2 = new OpenLinkedListNode<Recording.KeyFrame>(new Recording.KeyFrameK(KeyActions.PRESS, System.Windows.Forms.Keys.B, 1300));
            //n1.Next = n2;
            //var container = new RecordingModel.FocusContainer();
            //var t1 = new InfoTray(n1, null, focusedTray: container);
            //var t2 = new InfoTray(n2, t1.Model, focusedTray: container);
            //model.Keyframes.Add(t1);
            //model.Keyframes.Add(t2);

            Recording rec = new Recording();
            //rec.AddKeyFrame(new Recording.KeyFrameK(KeyActions.PRESS, System.Windows.Forms.Keys.A, 1200));
            //rec.AddKeyFrame(new Recording.KeyFrameK(KeyActions.PRESS, System.Windows.Forms.Keys.B, 1300));
            rec.AddKeyFrame(new Recording.KeyFrameM(MouseAction.WM_LBUTTONDOWN, 25, 55, 1200));
            rec.AddKeyFrame(new Recording.KeyFrameM(MouseAction.WM_LBUTTONUP, 25, 55, 1300));
            onRec(rec);
        }

        private void onRec(Recording recording)
        {
            //model.Keyframes.Clear()
            data = recording;
            model = new RecordingModel(data, new Button[] { btnAddM, btnAddK, btnDel, btnDn, btnUp});
            keyframesControl.DataContext = model;
        }

        #region deprecated tree
        //private class LibraryTreeRoot
        //{
        //    public ObservableCollection<LibraryTreeFolder> Folders { get; set; }

        //    public LibraryTreeRoot()
        //    {
        //        Folders = new ObservableCollection<LibraryTreeFolder>();
        //    }
        //}

        // todo keep own ref for edits!!
        //private LibraryTreeFolder newFolder(string header)
        //{
        //    var t = new LibraryTreeFolder() { Header = header };
        //    root.Folders.Add(t);
        //    return t;
        //}

        //// todo this does not work >> custom style overwriting w/ triggers?
        //private void treeItemClicked(object sender, MouseButtonEventArgs e)
        //{
        //    var i = (TreeViewItem)sender;
        //    var s = (LibraryTreeObject)i.DataContext;
        //    Console.WriteLine(s.Header);
        //}

        #endregion

        private void traysLostFocusDelegate()
        {
            model.TraysLostFocus();
        }

        private void traysLostFocus(object sender, MouseButtonEventArgs e)
        {
            traysLostFocusDelegate();
        }

        private void txtFileName_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            traysLostFocusDelegate();
        }

        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            traysLostFocusDelegate();

        }

        private void btnLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            traysLostFocusDelegate();

        }

        private void Util_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string UP = btnUp.Name;
            string DN = btnDn.Name;
            string ADDM = btnAddM.Name;
            string ADDK = btnAddK.Name;
            string DEL = btnDel.Name;

            var tray = model.FocusedTray.Focused;
            var mNode = tray.Model;
            var node = tray.Model.DataNode;

            var s = ((Button)sender).Name;
            if(s.Equals(UP))
            {
                //Console.WriteLine("u1");
                //Console.WriteLine(node.Next?.Value.GetTime());
                //Console.WriteLine(node.Previous?.Value.GetTime());
                if (node.Previous == null) return;

                // wait, just swap values???
                var n = node.Next; var mn = mNode.Next; 
                node.Next = node.Previous; mNode.Next = mNode.Prev;
                node.Previous = node.Previous.Previous; mNode.Prev = mNode.Prev.Prev;

                node.Next.Next = n; mNode.Next.Next = mn;
                node.Next.Previous = node; mNode.Next.Prev = mNode;

                if (node.Previous != null && node.Previous.Previous != null)
                    node.Previous.Previous.Next = node.Previous;
                if (node.Next != null && node.Next.Next != null)
                    node.Next.Next.Previous = node.Next;

                if (mNode.Prev != null && mNode.Prev.Prev != null)
                    mNode.Prev.Prev.Next = mNode.Prev;
                if (mNode.Next != null && mNode.Next.Next != null)
                    mNode.Next.Next.Prev = mNode.Next;

                var kfs = model.Keyframes;
                int i = kfs.IndexOf(tray);
                kfs.Remove(tray);
                kfs.Insert(i - 1, tray);

                var temp = node.Value.GetTime();
                node.Value.SetTime(node.Next.Value.GetTime());
                node.Next.Value.SetTime(temp);
                mNode.SetTime(mNode.DataNode.Value.GetTime());
                mNode.Next.SetTime(temp);

                //Console.WriteLine("u2");
            }
            else if(s.Equals(DN))
            {

                #region debug
                //Console.WriteLine("d1");

                //Action test = () =>
                //{
                //    Console.WriteLine("xx" + node.Value.GetTime());
                //    var tt = model.Keyframes.First().Model.DataNode;
                //    while (tt != null)
                //    {
                //        Console.WriteLine(tt.Value.GetTime());

                //        tt = tt.Next;
                //    }
                //    var ttt = model.Keyframes.First().Model;
                //    while (ttt != null)
                //    {
                //        Console.WriteLine(ttt.Time);

                //        ttt = ttt.Next;
                //    }
                //};
                //test();

                //Console.WriteLine("xx"+node.Value.GetTime());
                #endregion

                if (node.Next == null) return;

                var p = node.Previous; var mp = mNode.Prev;
                node.Previous = node.Next; mNode.Prev = mNode.Next;
                node.Next = node.Next.Next; mNode.Next = mNode.Next.Next;

                node.Previous.Previous = p; mNode.Prev.Prev = mp;
                node.Previous.Next = node; mNode.Prev.Next = mNode;

                if (node.Previous != null && node.Previous.Previous != null)
                    node.Previous.Previous.Next = node.Previous;
                if (node.Next != null && node.Next.Next != null)
                    node.Next.Next.Previous = node.Next;

                if (mNode.Prev!= null && mNode.Prev.Prev != null)
                    mNode.Prev.Prev.Next = mNode.Prev;
                if (mNode.Next != null && mNode.Next.Next != null)
                    mNode.Next.Next.Prev = mNode.Next;

                #region debug
                //Console.WriteLine("a" + node.Previous.Previous.Value.GetTime());
                //Console.WriteLine("a" + node.Previous.Value.GetTime());
                //Console.WriteLine("a" + node.Previous.Next.Value.GetTime());

                //Action test2 = () =>
                //{
                //    Console.WriteLine("+++");

                //    var tt = model.Keyframes.First().Model.DataNode;
                //    int j = 0;
                //    while (tt != null)
                //    {
                //        Console.WriteLine(j + " " + tt.Value.GetTime());

                //        tt = tt.Next;
                //        j++;
                //    }
                //    var ttt = model.Keyframes.First().Model;
                //    j = 0;
                //    while (ttt != null)
                //    {
                //        Console.WriteLine(j + " " + ttt.Time);

                //        ttt = ttt.Next;
                //        j++;
                //    }
                //};
                //test2();
                #endregion
                var kfs = model.Keyframes;
                int i = kfs.IndexOf(tray);
                kfs.Remove(tray);
                kfs.Insert(i + 1, tray);

                var temp = node.Value.GetTime();
                node.Value.SetTime(node.Previous.Value.GetTime());
                node.Previous.Value.SetTime(temp);
                mNode.Prev.SetTime(temp);
                mNode.SetTime(mNode.DataNode.Value.GetTime());

                #region debug
                //Console.WriteLine("+++");

                //var tt = model.Keyframes.First().Model.DataNode;
                //while (tt != null)
                //{
                //    Console.WriteLine(tt.Value.GetTime());

                //    tt = tt.Next;
                //}
                //var ttt = model.Keyframes.First().Model;
                //while (ttt != null)
                //{
                //    Console.WriteLine(ttt.Time);

                //    ttt = ttt.Next;
                //}

                //mNode = mNode.Prev.Prev;
                //Console.WriteLine(mNode.DataNode.Value.GetTime());
                //Console.WriteLine(mNode.Next.DataNode.Value.GetTime());
                ////Console.WriteLine(mNode.Next.Next.DataNode.Value.GetTime());

                //Console.WriteLine(mNode.DataNode.Value.GetTime());
                //Console.WriteLine(mNode.DataNode.Next.Value.GetTime());
                //Console.WriteLine(mNode.DataNode.Next.Next.Value.GetTime());
                #endregion
                //Console.WriteLine("d2");
            }
            else if (s.Equals(ADDM) || s.Equals(ADDK))
            {
                Recording.Keyframe k;
                var time = (node.Value.GetTime() + node.Next.Value.GetTime()) / 2;
                if (s.Equals(ADDM)) k = new Recording.KeyFrameM(MouseAction.WM_LBUTTONDOWN, 0, 0, time);
                else k = new Recording.KeyFrameK(KeyActions.PRESS, Key.X, time);

                var newNode = new OpenLinkedListNode<Recording.Keyframe>(k);
                if (node.Next != null) node.Next.Previous = newNode;
                newNode.Next = node.Next;
                node.Next = newNode;
                newNode.Previous = node;

                var mNext = mNode.Next;
                InfoTray newTray = new InfoTray(newNode, mNode, model.FocusedTray);
                if (mNext != null) mNext.Prev = newTray.Model;
                newTray.Model.Next = mNext;

                int i = model.Keyframes.IndexOf(tray);
                if (i == model.Keyframes.Count-1) // last tray
                {
                    model.Keyframes.Add(newTray);
                }
                else
                {
                    model.Keyframes.Insert(i + 1, newTray);
                }
            }
            else if (s.Equals(DEL))
            {

            }
        }
    }

    public class RecordingModel
    {
        public class FocusContainer
        {
            private InfoTray focused;
            private RecordingModel parent;

            public FocusContainer(RecordingModel p)
            {
                parent = p;
            }

            public InfoTray Focused
            {
                get
                {
                    return focused;
                }
                set
                {
                    parent.SetUtilsEnable(value != null);
                    focused = value;
                }
            }

            public Action FocusChanged = () => { };
        }

        public FocusContainer FocusedTray;
        private Button[] utils;
        public ObservableCollection<InfoTray> Keyframes { get; set; }

        public RecordingModel(Recording rec, Button[] utils)
        {
            this.utils = utils;
            FocusedTray = new FocusContainer(this);
            Keyframes = new ObservableCollection<InfoTray>();

            OpenLinkedListNode<Recording.Keyframe> curr = rec.Keyframes.First();
            KeyframeModel prev = null;
            while (curr != null)
            {
                var tray = new InfoTray(curr, prev, FocusedTray);
                Keyframes.Add(tray);
                prev = tray.Model;
                curr = curr.Next;
            }
        }

        public void SetUtilsEnable(bool enable)
        {
            foreach(var btn in utils)
            {
                btn.IsEnabled = enable;
            }
        }

        public void TraysLostFocus()
        {
            var f = FocusedTray;
            f.FocusChanged();
            f.Focused = null;
            f.FocusChanged = () => { };
        }

        //public RecordingModel(Recording rec) : this()
        //{
        //    OpenLinkedListNode<Recording.KeyFrame> curr = rec.Keyframes.First();
        //    KeyFrameModel prev = null;
        //    while(curr != null)
        //    {
        //        var tray = new InfoTray(curr, prev, focusedTray);
        //        Keyframes.Add(tray);
        //        prev = tray.Model;
        //        curr = curr.Next;
        //    }

        //}
    }

}