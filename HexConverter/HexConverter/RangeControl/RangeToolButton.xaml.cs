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

namespace HexConverter.RangeControl
{
    /// <summary>
    /// RangeToolButton.xaml 的交互逻辑
    /// </summary>
    public partial class RangeToolButton : UserControl
    {
        public RangeToolButton()
        {
            InitializeComponent();
        }


        public event EventHandler<RoutedEventArgs> AddClick;
        public event EventHandler<RoutedEventArgs> RemoveClick;
        
        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddClick?.Invoke(this,e);
        }

        private void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveClick?.Invoke(this, e);
        }
    }
}
