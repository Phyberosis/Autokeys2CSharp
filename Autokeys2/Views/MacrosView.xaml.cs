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

            model = new RecordingModel();
            keyframesControl.DataContext = model;
            model.Keyframes.Add(new InfoTray(1200, "Test", "down at 12, 12", null));
            model.Keyframes.Add(new InfoTray(1200, "Test", "up at 12, 12", null));
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
    }

    public class RecordingModel
    {
        public ObservableCollection<InfoTray> Keyframes { get; set; }

        public RecordingModel()
        {
            Keyframes = new ObservableCollection<InfoTray>();
        }

        public RecordingModel(Recording rec) : this()
        {
            LinkedListNode<Recording.KeyFrame> curr = rec.Keyframes.First;
            while(curr != null)
            {
                Recording.KeyFrame k = curr.Value;
                Keyframes.Add(new InfoTray(k.GetTime(), k.GetInfo(), k.GetDescription(), curr));
                curr = curr.Next;
            }

        }
    }

}