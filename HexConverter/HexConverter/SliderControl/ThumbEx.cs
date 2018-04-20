using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace HexConverter.SliderControl
{
    public class HexSliderRangeChangedEventArgs:EventArgs
    {
        public int RangeIndex { get; set; }
        public int OriginalValue { get; set; }
        public int NewValue { get; set; }

    }
}
