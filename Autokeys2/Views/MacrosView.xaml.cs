using Autokeys2.ViewModels;
using Autokeys2.Views.Trays;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    //internal class MacroFolder
    //{
    //    public string Name;
    //    public List<MacroItem> Items;

    //    public MacroFolder(string name)
    //    {
    //        Name = name;
    //        Items = new List<MacroItem>();
    //    }
    //}

    public partial class MacrosView : UserControl
    {
        List<MacroItem> treeData;

        public MacrosView()
        {
            InitializeComponent();

            treeData = new List<MacroItem>();
            treeData.Add(new MacroItem("1"));

            tree.DataContext = treeData;

            Console.WriteLine("here");

            Task.Delay(0).ContinueWith((t) =>
           {
               var i = 2;
               while(true)
               {
                   this.Dispatcher.Invoke(() =>
                   {
                       treeData.Add(new MacroItem(i.ToString()));
                   });
                   Thread.Sleep(500);
                   i++;
               }
           });
        }
    }
}