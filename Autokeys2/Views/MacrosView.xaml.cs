using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Events;
using Recordings;
using System.Linq;
using FileHandler;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Runtime.InteropServices;

namespace Autokeys2.Views
{
    public partial class MacrosView : UserControl
    {
        private Recording data;
        private RecordingModel model;

        private EventHandle<Recording> recLoaded;

        public MacrosView()
        {
            InitializeComponent();

            EventsBuiltin.RegisterListener<Recording>(EventID.REC,
                (r) =>
                {
                    this.Dispatcher.Invoke(() => { onRec(r); });
                });

            recLoaded = EventsBuiltin.RegisterEvent<Recording>(EventID.REC_LOADED);

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

            //Recording rec = new Recording();
            ////rec.AddKeyFrame(new Recording.KeyFrameK(KeyActions.PRESS, System.Windows.Forms.Keys.A, 1200));
            ////rec.AddKeyFrame(new Recording.KeyFrameK(KeyActions.PRESS, System.Windows.Forms.Keys.B, 1300));
            //rec.AddKeyframe(MouseAction.WM_LBUTTONDOWN, 25, 1, 1200);
            //rec.AddKeyframe(MouseAction.WM_LBUTTONUP, 35, 2, 1300);
            //onRec(rec);
        }

        private void onRec(Recording recording)
        {
            //model.Keyframes.Clear()
            data = recording;
            model = new RecordingModel(data, new Button[] { btnDel, btnDn, btnUp }, this);
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
            Focus();
            traysLostFocusDelegate();
        }

        private void txtFileName_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            traysLostFocusDelegate();
        }

        private void txtFileName_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            model.FileName = txtFileName.Text;
        }

        private void txtFileName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                model.FileName = txtFileName.Text;
            }else if(e.Key == Key.Escape)
            {
                model.ResetFileName();
            }
        }

        private void btnSave_MouseDown(object sender, MouseButtonEventArgs e)
        {
            traysLostFocusDelegate();
            var name = txtFileName.Text;
            if (name == null || name  == string.Empty)
            {
                Task.Delay(0).ContinueWith((t) => {
                    Dispatcher.Invoke(() => {
                        txtFileName.Focus();
                    });
                });
                return;
            }

            if (name.EndsWith(".aks")) name = name.Substring(0, name.Length - 4);
            model.FileName = name;

            var l = Librarian.I;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.InitialDirectory = l.Path;
            dialog.Filter = "Autokeys Save|*.aks";
            dialog.FileName = l.CompileName(model.FileName);
            dialog.Title = "Saving "+dialog.FileName;
            var result = dialog.ShowDialog();
            
            if (result == true)
            {
                var fs = dialog.OpenFile();

                l.Save(data, MainWindow.SettingsModel.GetSettings(), fs);
                fs.Close();
            }

            btnSave.Focus();
        }

        private void btnLoad_MouseDown(object sender, MouseButtonEventArgs e)
        {
            traysLostFocusDelegate();

            var l = Librarian.I;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = l.Path;
            dialog.Filter = "Autokeys Save|*.aks";
            dialog.Title = "Open";
            var result = dialog.ShowDialog();

            if(result == true)
            {
                var fs = dialog.OpenFile();
                var f = l.Load(dialog.SafeFileName, fs);
                fs.Close();

                var rec = f.Recording;
                data = rec;
                recLoaded.Notify(rec);
                model.FileName = f.FileName;
                model.LoadRecording(rec);
                MainWindow.SettingsModel.LoadSettings(f.Settings);
            }
        }
        private void Util_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            string UP = btnUp.Name;
            string DN = btnDn.Name;
            string ADDM = btnAddM.Name;
            string ADDK = btnAddK.Name;
            string DEL = btnDel.Name;
            string DIS = btnDis.Name;

            var tray = model.FocusedTray.Focused;
            var kf = tray?.GetKeyframe();
            //var mNode = tray.Model;
            //var node = tray.Model.DataNode;
            Focus();

            var s = ((Button)sender).Name;
            if (s.Equals(UP))
            {
                const int os = -2;
                if (!data.KeyframeOffsetExists(kf, os + 1)) return;

                var p = data.GetLinkedWithOffset(kf, os);
                data.ShiftKeyframe(kf, p);

                var i = model.Keyframes.IndexOf(tray);
                model.Keyframes.Remove(tray);
                model.Keyframes.Insert(i - 1, tray);
            }
            else if (s.Equals(DN))
            {
                const int os = 1;
                if (!data.KeyframeOffsetExists(kf, os)) return;
                
                var p = data.GetLinkedWithOffset(kf, os);
                data.ShiftKeyframe(kf, p);

                var i = model.Keyframes.IndexOf(tray);
                model.Keyframes.Remove(tray);
                model.Keyframes.Insert(i + 1, tray);
            }
            else if (s.Equals(ADDM) || s.Equals(ADDK))
            {
                Recording.Keyframe nk;
                if (s.Equals(ADDM))
                    nk = data.MakeDefaultM(kf);
                else
                    nk = data.MakeDefaultK(kf);
                var i = tray != null ? model.Keyframes.IndexOf(tray) + 1 : model.Keyframes.Count;
                var nt = InfoTray.BuildNew(nk, model.FocusedTray);

                model.Keyframes.Insert(i, nt);
                nt.SetAsFocusedTray();

                //Console.WriteLine(model.Keyframes.Count);
            }
            else if (s.Equals(DEL))
            {
                var c = model.Keyframes.Count;
                if (c > 1)
                {
                    var i = model.Keyframes.IndexOf(tray);
                    if (c == 2) i = i == 0 ? 1 : 0;
                    else i = i == 0 ? 1 : i == c - 1 ? c - 2 : i + 1;

                    var t = model.Keyframes.ElementAt(i);
                    t.SetAsFocusedTray();
                }
                else
                {
                    traysLostFocusDelegate();
                }

                data.DeleteKeyframe(kf);
                model.Keyframes.Remove(tray);
            }
            else if (s.Equals(DIS))
            {
                data.Distribute(MainWindow.SettingsModel.GetSpeed());
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

        private MacrosView ui;

        private string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                fileName = value!=null? value : string.Empty;
                ui?.Dispatcher.Invoke(() => { ui.txtFileName.Text = value; });
            }
        }

        public FocusContainer FocusedTray;
        private Button[] utils;
        public ObservableCollection<InfoTray> Keyframes { get; set; }

        public RecordingModel(Recording rec, Button[] utils, MacrosView ui)
        {
            this.ui = ui;
            this.utils = utils;
            FocusedTray = new FocusContainer(this);
            Keyframes = new ObservableCollection<InfoTray>();

            LoadRecording(rec);
        }

        public void LoadRecording(Recording rec)
        {
            Keyframes.Clear();
            FocusedTray = new FocusContainer(this);
            foreach (var kf in rec.Keyframes)
            {
                var tray = InfoTray.BuildNew(kf, FocusedTray);
                Keyframes.Add(tray);
            }
        }

        public void ResetFileName()
        {
            FileName = fileName;
        }

        public void SetUtilsEnable(bool enable)
        {
            foreach (var btn in utils)
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