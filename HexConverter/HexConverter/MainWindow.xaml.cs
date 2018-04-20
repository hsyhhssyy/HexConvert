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
using HexConverter.RangeControl;
using HexConverter.SliderControl;

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

            ResetRangePanel();

            hexSlider.RangeChanged += HexSlider_RangeChanged;

            _suppressTextChange = false;
        }

        private void ResetRangePanel()
        {
            panRangeControls.Children.Clear();
            
            panRangeControls.Children.Insert(0, new RangePanel());

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

            ResetRangePanel();
        }


        private void ToolButton_AddClick(object sender, RoutedEventArgs e)
        {
            panRangeControls.Children.Insert(panRangeControls.Children.Count - 2, new RangePanel());
            var val = hexSlider.Ranges.Last();
            hexSlider.Ranges[hexSlider.Ranges.Count - 1] = val / 2;
            hexSlider.Ranges.Add(val - val / 2);


            RefreshControls(hexSlider);
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
        
    }
}
