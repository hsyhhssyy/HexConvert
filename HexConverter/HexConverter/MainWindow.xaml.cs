using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
using System.Xml.Linq;
using System.Xml.XPath;
using HexConverter.RangeControl;
using HexConverter.SliderControl;
using Microsoft.Win32;

namespace HexConverter
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            cboMaxium.SelectedIndex = 3;

            ResetRangePanel(true);

            hexSlider.RangeChanged += HexSlider_RangeChanged;

            _suppressTextChange = false;
        }

        private void ResetRangePanel(bool createDefault)
        {
            panRangeControls.Children.Clear();

            if (createDefault)
            {
                panRangeControls.Children.Insert(0, new RangePanel());
            }

            var toolButton = new RangeToolButton();
            toolButton.AddClick += ToolButton_AddClick;
            toolButton.RemoveClick += ToolButton_RemoveClick;
            panRangeControls.Children.Add(toolButton);
        }


        public Int64 ValueHold=0;

        private bool _suppressTextChange = true;

        public void RefreshControls(Control source)
        {
            _suppressTextChange = true;

            if (!Equals(source, txtHexValue))
            {
                txtHexValue.BorderBrush = Brushes.Black;
                txtHexValue.Text = ValueHold.ToString("X");
            }

            if (!Equals(source, txtDecValue))
            {
                txtDecValue.BorderBrush = Brushes.Black;
                txtDecValue.Text = ValueHold.ToString();
            }

            if (!Equals(source, txtBinValue))
            {
                txtBinValue.BorderBrush = Brushes.Black;
                txtBinValue.Text = Convert.ToString(ValueHold, 2);
            }

            if (!Equals(source, hexSlider))
            {
                hexSlider.ValueHold = ValueHold;
            }

            for (var index = 0; index < panRangeControls.Children.Count-1; index++)
            {
                var child = (RangePanel) panRangeControls.Children[index];
                child.Color = hexSlider.GetRangeColor(index);
            }

            if (!(source is RangePanel))
            {
                var val = ValueHold;
                for (var i = hexSlider.Ranges.Count - 1; i >= 0; i--)
                {
                    var value = hexSlider.Ranges[i];

                    var val2 = (val >> value)<<value ;

                    var child = (RangePanel)panRangeControls.Children[i];
                    child.ValueHold = val-val2;

                    val = val >> value;
                }
            }
            
            _suppressTextChange = false;
        }

        private void TxtHexValue_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChange)
            {
                return;
            }
            try
            {
                var dec = Convert.ToInt64(txtHexValue.Text, 16);
                txtHexValue.BorderBrush = Brushes.Black;
                ValueHold = dec;
            }
            catch (Exception)
            {
                txtHexValue.BorderBrush = Brushes.Red;
                return;
            }

            RefreshControls(txtHexValue);
        }

        private void TxtDecValue_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChange)
            {
                return;
            }
            try
            {
                var dec = Convert.ToInt64(txtDecValue.Text,10);
                txtDecValue.BorderBrush = Brushes.Black;
                ValueHold = dec;
            }
            catch (Exception)
            {
                txtDecValue.BorderBrush = Brushes.Red;
                return;
            }

            RefreshControls(txtDecValue);
        }

        private void TxtBinValue_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChange)
            {
                return;
            }
            try
            {
                var dec = Convert.ToInt64(txtBinValue.Text, 2);
                txtBinValue.BorderBrush = Brushes.Black;
                ValueHold = dec;
            }
            catch (Exception)
            {
                txtBinValue.BorderBrush = Brushes.Red;
                return;
            }

            RefreshControls(txtBinValue);

        }

        private void CboMaxium_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            hexSlider.Ranges.Clear();
            hexSlider.Ranges.Add(Convert.ToInt32(((ComboBoxItem) cboMaxium.SelectedItem).Content));

            ResetRangePanel(true);
        }


        private void ToolButton_AddClick(object sender, RoutedEventArgs e)
        {
            var newElement = new RangePanel();
            newElement.ValueChanged += RangePanel_ValueChanged;
            panRangeControls.Children.Insert(panRangeControls.Children.Count - 2, newElement);
            var val = hexSlider.Ranges.Last();
            hexSlider.Ranges[hexSlider.Ranges.Count - 1] = val / 2;
            hexSlider.Ranges.Add(val - val / 2);


            RefreshControls(hexSlider);
        }

        private void RangePanel_ValueChanged(object sender, EventArgs e)
        {
            long val = 0;
            for (var i = hexSlider.Ranges.Count - 1; i >= 0; i--)
            {
                var value = hexSlider.Ranges[i];
                
                var child = (RangePanel)panRangeControls.Children[hexSlider.Ranges.Count-i-1];
                val=(val<<value)+child.ValueHold;
            }

            ValueHold = val;
            RefreshControls(sender as RangePanel);
        }

        private void ToolButton_RemoveClick(object sender, RoutedEventArgs e)
        {
            panRangeControls.Children.RemoveAt(panRangeControls.Children.Count-2);

            var newVal = hexSlider.Ranges[hexSlider.Ranges.Count - 2] + hexSlider.Ranges[hexSlider.Ranges.Count - 1];
            
            hexSlider.Ranges.RemoveAt(hexSlider.Ranges.Count - 1);
            hexSlider.Ranges[hexSlider.Ranges.Count - 1] = newVal;

            RefreshControls(hexSlider);
        }

        private void HexSlider_RangeChanged(object sender, HexSliderRangeChangedEventArgs e)
        {
            RefreshControls(hexSlider);
        }

        private void BtnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog save=new SaveFileDialog();
            save.Filter = "配置文件|*.xml";
            save.ShowDialog();

            if (!String.IsNullOrWhiteSpace(save.FileName))
            {
                XDocument doc=new XDocument();
                XElement root = new XElement("HexConvert");
                doc.Add(root);

                for (int index = 0; index < hexSlider.Ranges.Count; index++)
                {
                    var child = (RangePanel)panRangeControls.Children[index];
                    XElement ele=new XElement("Range");
                    ele.Add(new XAttribute("value", hexSlider.Ranges[index]));
                    ele.Add(new XAttribute("color", child.Color.ToString()));
                    ele.Add(new XCData(child.Comment));
                    root.Add(ele);
                }

                XElement comm = new XElement("Comment");
                comm.Add(new XCData(txtComment.Text));
                root.Add(comm);

                XElement hold = new XElement("Holding");
                hold.Add(new XAttribute("value", ValueHold));
                hold.Add(new XAttribute("maxium", cboMaxium.SelectedIndex));
                root.Add(hold);

                doc.Save(save.FileName);

                MessageBox.Show("保存完成");
            }
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd=new OpenFileDialog();
            ofd.Filter = "配置文件|*.xml";
            ofd.ShowDialog();

            if (!String.IsNullOrWhiteSpace(ofd.FileName))
            {
                    XDocument doc = XDocument.Load(ofd.FileName);

                    hexSlider.Ranges.Clear();
                    ResetRangePanel(false);

                    foreach (var element in doc.XPathSelectElements(@"HexConvert/Range"))
                    {
                        hexSlider.Ranges.Add(Convert.ToInt32(element.Attribute("value").Value));
                        var pan = new RangePanel();
                        pan.Color= (Color)ColorConverter.ConvertFromString(element.Attribute("color").Value);
                        pan.Comment = element.Value;
                        pan.ValueChanged += RangePanel_ValueChanged;

                    panRangeControls.Children.Insert(panRangeControls.Children.Count - 1, pan);
                    }

                    var comment = doc.XPathSelectElement(@"HexConvert/Comment");
                    txtComment.Text = comment.Value;

                var hold = doc.XPathSelectElement(@"HexConvert/Holding");
                ValueHold =Convert.ToInt64(hold.Attribute("value").Value);

                RefreshControls(this);
            }
        }
    }
}
