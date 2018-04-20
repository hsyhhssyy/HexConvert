using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace HexConverter.SliderControl
{
    /// <summary>
    /// 这个类隐藏了ThumbBase里的私有互访方法
    /// </summary>
    public class HexSliderThumb : HexSlider.HexSliderThumbBase
    {
        public HexSliderThumb(Rectangle leftRect, Rectangle rightRect, HexSlider parentSlider) : base(leftRect, rightRect, parentSlider)
        {
        }
    }

    public partial class HexSlider
    {
        public class HexSliderThumbBase : Thumb
        {
            private readonly Rectangle _leftRect;
            private readonly Rectangle _rightRect;
            private readonly HexSlider _parentSlider;

            public HexSliderThumbBase(Rectangle leftRect, Rectangle rightRect, HexSlider parentSlider)
            {
                _leftRect = leftRect;
                _rightRect = rightRect;
                _parentSlider = parentSlider;
                this.DragDelta += ThumbEx_DragDelta;
                this.DragStarted += ThumbEx_DragStarted;
                this.DragCompleted += ThumbEx_DragCompleted;
            }

            private void ThumbEx_DragStarted(object sender, DragStartedEventArgs e)
            {

            }

            private void ThumbEx_DragDelta(object sender, DragDeltaEventArgs e)
            {
                if ((_leftRect.Width + e.HorizontalChange) >= 0 && (_rightRect.Width - e.HorizontalChange) >= 0)
                {
                    _leftRect.Width += e.HorizontalChange;
                    _rightRect.Width -= e.HorizontalChange;

                    var sliderLeft = Canvas.GetLeft(this);
                    sliderLeft += e.HorizontalChange;
                    Canvas.SetLeft(this, sliderLeft);
                }
            }

            private void ThumbEx_DragCompleted(object sender, DragCompletedEventArgs e)
            {
                if (_leftRect.Parent is StackPanel ancestor)
                {
                    var newLeft = _leftRect.Width / _parentSlider.ContentWidth * _parentSlider.Maxium;
                    var newRight = _rightRect.Width / _parentSlider.ContentWidth * _parentSlider.Maxium;

                    var roundedLeft = (int)Math.Round(newLeft);
                    var roundedRight = (int)Math.Round(newRight);
                    if (roundedLeft < newLeft && roundedRight < newRight)
                    {
                        roundedRight++;
                    }

                    if (roundedLeft > newLeft && roundedRight > newRight)
                    {
                        roundedRight--;
                    }

                    _leftRect.Width = roundedLeft * _parentSlider.ContentWidth / _parentSlider.Maxium;
                    _rightRect.Width = roundedRight * _parentSlider.ContentWidth / _parentSlider.Maxium;

                    var point = _leftRect.TransformToAncestor(ancestor).Transform(new Point(0, 0));
                    Canvas.SetLeft(this, point.X + _leftRect.Width);

                    _parentSlider.DragCompleted(this, roundedLeft, roundedRight);
                }
            }
        }
    }
}