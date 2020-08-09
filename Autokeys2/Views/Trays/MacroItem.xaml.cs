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

namespace Autokeys2.Views.Trays
{
    public partial class MacroItem : UserControl
    {

        public ObservableCollection<MacroItem> Children = new ObservableCollection<MacroItem>();
        public string Header;

        public MacroItem(string header)
        {
            InitializeComponent();

            Header = header;
        }
    }
}
