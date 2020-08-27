using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();


            bindingContainer d = new bindingContainer();
            d.textdata = "asdf";
            d.mylist.Add(new UserControl1());
            d.mylist.Add(new UserControl1());
            d.mylist.Add(new UserControl1());
            d.mylist.Add(new UserControl1());
            d.mylist.Add(new UserControl1());
            d.mylist.Add(new UserControl1());
            itemsList.DataContext = d;
        }
    }

    public class bindingContainer
    {
        public string textdata { get; set; }
        public ObservableCollection<UserControl1> mylist { get; set; }

        public bindingContainer()
        {
            mylist = new ObservableCollection<UserControl1>();
        }
    }

}
