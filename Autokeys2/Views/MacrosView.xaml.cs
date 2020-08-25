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
            model = new RecordingModel(data);
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

        private void traysLostFocus(object sender, MouseButtonEventArgs e)
        {
            model.TraysLostFocus();
        }

        private void txtFileName_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            model.TraysLostFocus();

        }

        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            model.TraysLostFocus();

        }

        private void btnLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            model.TraysLostFocus();

        }
    }

    public class RecordingModel
    {
        public class FocusContainer
        {
            public object Focused = null;
            public Action FocusChanged = () => { };
        }

        private FocusContainer focusedTray = new FocusContainer();
        public ObservableCollection<InfoTray> Keyframes { get; set; }

        public RecordingModel()
        {
            Keyframes = new ObservableCollection<InfoTray>();
        }

        public void TraysLostFocus()
        {
            var f = focusedTray;
            f.FocusChanged();
            f.Focused = null;
            f.FocusChanged = () => { };
        }

        public RecordingModel(Recording rec) : this()
        {
            OpenLinkedListNode<Recording.KeyFrame> curr = rec.Keyframes.First();
            KeyFrameModel prev = null;
            while(curr != null)
            {
                var tray = new InfoTray(curr, prev, focusedTray);
                Keyframes.Add(tray);
                prev = tray.Model;
                curr = curr.Next;
            }

        }
    }

}