using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autokeys2.Views;
using Autokeys2.Views.Trays;

namespace Autokeys2.ViewModels
{
    interface IMacrosTreeViewModel: INotifyPropertyChanged
    {
        ObservableCollection<MacroItem> Children { get; }
        bool HasDummyChild { get; }
        bool IsExpanded { get; set; }
        bool IsSelected { get; set; }
        MacroItem Parent { get; }
    }
}
