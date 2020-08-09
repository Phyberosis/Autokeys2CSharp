using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Autokeys2.ViewModels
{
    public class TestViewModel : INotifyPropertyChanged
    {
        public string Header;
        public List<TestViewModel> Children;

        public TestViewModel(string name)
        {
            Header = name;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void onPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
