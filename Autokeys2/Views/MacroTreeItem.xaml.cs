using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
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
    public partial class MacrosTreeItem : UserControl
    {
        public string Header { get; set; }
        public ObservableCollection<MacrosTreeItem> Children { get; set; }

        public MacrosTreeItem()
        {
            Children = new ObservableCollection<MacrosTreeItem>();
        }
    }
}