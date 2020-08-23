using Recordings;
using System;
using System.Collections.Generic;
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

namespace Autokeys2.Views
{
    /// <summary>
    /// Interaction logic for InfoTray.xaml
    /// </summary>

    public partial class InfoTray : UserControl
    {
        private LinkedListNode<Recording.KeyFrame> dataNode;
        private KeyFrameModel model;

        public InfoTray(long time, string info, string desc, LinkedListNode<Recording.KeyFrame> node)
        {
            InitializeComponent();
            model = new KeyFrameModel();
            model.Info = info;
            model.Description = desc;
            model.Time = ((float)time / 1000).ToString();
            dataNode = node;
            this.DataContext = model;
        }

        //public InfoTray(string info, string description)
        //{
        //    InitializeComponent();
        //}
    }

    public class KeyFrameModel
    {
        public string Info { get; set; }
        public string Description { get; set; }
        public string Time { get; set; }
    }

}
