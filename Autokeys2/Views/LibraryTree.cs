using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autokeys2.Views
{
    public abstract class LibraryTreeObject
    {
        public string Header { get; set; }
    }


    public class LibraryTreeFolder : LibraryTreeObject
    {
        public ObservableCollection<LibraryTreeItem> Children { get; set; }

        public LibraryTreeFolder()
        {
            Children = new ObservableCollection<LibraryTreeItem>();
        }
    }

    public class LibraryTreeItem : LibraryTreeObject
    {

    }
}
