using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HexConverter.SliderControl
{
    /// <summary>
    /// HexSlider.xaml 的交互逻辑
    /// </summary>
    public partial class HexSlider 
    {
        public HexSlider()
        {
            InitializeComponent();
            _rectangles = new List<Rectangle>();
            _thumbExs = new List<HexSliderThumbBase>();
            _labels=new List<Label>();
            _colors=new Dictionary<int, SolidColorBrush>();
            Ranges = new ObservableCollection<int>() { 6, 5, 5 };
            Ranges.CollectionChanged += Ranges_CollectionChanged;

            _valueHold = 0;

            SizeChanged += HexSlider_SizeChanged;
            BuildRange();
        }
        
        private readonly List<Rectangle> _rectangles;
        private readonly List<Label> _labels;
        private Random _colorRandom;
        private long _valueHold;
        private int Maxium { get; set; }
        
        private readonly List<HexSliderThumbBase> _thumbExs;
        private readonly Dictionary<int, SolidColorBrush> _colors;

        private double ContentWidth => ActualWidth < 30 ? 0 : ActualWidth - 30;
        public ObservableCollection<int> Ranges { get;  }

        public event EventHandler<HexSliderRangeChangedEventArgs> RangeChanged;
        
        public Int64 ValueHold
        {
            get => _valueHold;
            set
            {
                _valueHold = value; 
                BuildRange();
            }
        }

        #region 重新绘图

        private void Ranges_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            BuildRange();
        }

        private void HexSlider_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BuildRange();
        }

        private void BuildRange()
        {
            Maxium = Ranges.Aggregate(0, (a, b) => a + b);

            _labels.Clear();
            labsGrid.Children.Clear();
            labsGrid.ColumnDefinitions.Clear();
            for (int index = 0; index < Maxium; index++)
            {
                var col = new ColumnDefinition {Width = new GridLength(1, GridUnitType.Star)};
                labsGrid.ColumnDefinitions.Add(col);
                var lab = new Label
                {
                    Margin = new Thickness(-5, 0, -5, 0),
                    Content = '0',
                    FontSize = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                };

                _labels.Add(lab);
                Grid.SetColumn(lab, index);
                labsGrid.Children.Add(lab);
            }

            var binFormat = Convert.ToString(_valueHold, 2).Reverse().ToList();
            for (int index = Maxium - 1; index >= 0; index--)
            {
                var label = _labels[Maxium-1-index];
                if (binFormat.Count > index)
                {
                    label.Content = binFormat[index];
                }
            }

            if (Ranges.Count <= 0)
            {
                BuildSingleRange();
            }
            else
            {
                BuildMultiRange();
            }
        }

        private void BuildSingleRange()
        {
            RangeContainer.Children.Clear();

            var item = Ranges.FirstOrDefault();

            if (item !=0)
            {
                var rect = new Rectangle
                {
                    Width = ContentWidth,
                    Margin = new Thickness(0, 40, 0, 40),
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                    Fill = GetColor(0)
                };


                RangeContainer.Children.Add(rect);
            }
            else
            {

            }

        }
        
        private void BuildMultiRange()
        {
            RangeContainer.Children.Clear();
            _rectangles.Clear();
            _thumbExs.Clear();
            ThumbContainer.Children.Clear();

            for (var index = 0; index < Ranges.Count; index++)
            {
                var value = Ranges[index];
                var rectWidth = value * ContentWidth / Maxium;

                var rect = new Rectangle
                {
                    Width = rectWidth,
                    Margin = new Thickness(0, 0, 0, 0),
                    Stroke = Brushes.Transparent,
                    StrokeThickness = 0,
                    Fill = GetColor(index)
                };

                RangeContainer.Children.Add(rect);
                _rectangles.Add(rect);
            }

            //滑块起始位置
            var leftPosition = 0d;

            //构造滑块
            for (int i = 0; i < _rectangles.Count - 1; i++)
            {
                var leftRect = _rectangles[i];
                var rightRect = _rectangles[i + 1];

                leftPosition += leftRect.Width;

                var thumb = new HexSliderThumb(leftRect, rightRect,this);

                Canvas.SetLeft(thumb, leftPosition);

                ThumbContainer.Children.Add(thumb);
                _thumbExs.Add(thumb);
            }
        }

        #endregion

        public Color GetRangeColor(int index)
        {
            return _colors[index].Color;
        }

        private SolidColorBrush GetColor(int index)
        {
            if (!_colors.ContainsKey(index))
            {
                _colors[index] = new SolidColorBrush(GetRandomColor());
            }
            return _colors[index];
        }


        private Color GetRandomColor()
        {
            if (_colorRandom == null)
            {
                long tick = DateTime.Now.Ticks;
                _colorRandom = new Random((int)(tick & 0xffffffffL) | (int)(tick >> 32));
            }

            int r = _colorRandom.Next(255);
            int g = _colorRandom.Next(255);
            int b = _colorRandom.Next(255);
            b = (r + g > 400) ? r + g - 400 : b;
            b = (b > 255) ? 255 : b;
            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }

        private void DragCompleted(HexSliderThumbBase hexSliderThumbBase,int leftValue,int rightValue)
        {
            var index = _thumbExs.IndexOf(hexSliderThumbBase);

            HexSliderRangeChangedEventArgs e1=null;
            if (Ranges[index] != leftValue)
            {
                e1 = new HexSliderRangeChangedEventArgs()
                {
                    NewValue = leftValue,
                    OriginalValue = Ranges[index],
                    RangeIndex = index
                };
            }
            
            HexSliderRangeChangedEventArgs e2 = null;
            if (Ranges[index] != leftValue)
            {
                e2 = new HexSliderRangeChangedEventArgs()
                {
                    NewValue = rightValue,
                    OriginalValue = Ranges[index + 1],
                    RangeIndex = index + 1
                };
            }

            Ranges[index] = leftValue;
            Ranges[index + 1] = rightValue;

            if (e1 != null)
            {
                RangeChanged?.Invoke(this, e1);
            }
            if (e2 != null)
            {
                RangeChanged?.Invoke(this, e2);
            }
        }
    }
}
