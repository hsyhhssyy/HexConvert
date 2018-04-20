using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HexConverter.RangeControl
{
    /// <summary>
    /// RangePanel.xaml 的交互逻辑
    /// </summary>
    public partial class RangePanel : UserControl
    {
        public RangePanel()
        {
            InitializeComponent();
            _suppressTextChange = false;
        }

        public Color Color
        {
            get
            {
                if (rectHeader.Fill is SolidColorBrush)
                {
                    return ((SolidColorBrush) rectHeader.Fill).Color;
                }
                return Colors.Transparent;
            }
            set => rectHeader.Fill = new SolidColorBrush(value);
        }

        public long ValueHold
        {
            get => _valueHold;
            set
            {
                _valueHold = value;
                RefreshControls(null);
            }
        }
        public String Comment
        {
            get => txtComment.Text;
            set => txtComment.Text = value;
        }

        public event EventHandler ValueChanged;

        private bool _suppressTextChange = true;
        private long _valueHold;

        private void RefreshControls(Control source)
        {
            _suppressTextChange = true;

            if (!Equals(source, txtHexValue))
            {
                txtHexValue.BorderBrush = Brushes.Black;
                txtHexValue.Text = _valueHold.ToString("X");
            }

            if (!Equals(source, txtDecValue))
            {
                txtDecValue.BorderBrush = Brushes.Black;
                txtDecValue.Text = _valueHold.ToString();
            }

            if (!Equals(source, txtBinValue))
            {
                txtBinValue.BorderBrush = Brushes.Black;
                txtBinValue.Text = Convert.ToString(_valueHold, 2);
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
                _valueHold = dec;
            }
            catch (Exception)
            {
                txtHexValue.BorderBrush = Brushes.Red;
                return;
            }

            RefreshControls(txtHexValue);
            ValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void TxtDecValue_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChange)
            {
                return;
            }
            try
            {
                var dec = Convert.ToInt64(txtDecValue.Text, 10);
                txtDecValue.BorderBrush = Brushes.Black;
                _valueHold = dec;
            }
            catch (Exception)
            {
                txtDecValue.BorderBrush = Brushes.Red;
                return;
            }

            RefreshControls(txtDecValue);
            ValueChanged?.Invoke(this, EventArgs.Empty);
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
                _valueHold = dec;
            }
            catch (Exception)
            {
                txtBinValue.BorderBrush = Brushes.Red;
                return;
            }

            RefreshControls(txtBinValue);
            ValueChanged?.Invoke(this, EventArgs.Empty);

        }

        private void RectHeader_OnMouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
