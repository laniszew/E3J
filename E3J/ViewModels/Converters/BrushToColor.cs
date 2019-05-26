using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace E3J.ViewModels.Converters
{
    [ValueConversion(typeof(Brush), typeof(Color))]
    public class BrushToColor : IValueConverter
    {
        private static readonly Color DEFAULT_COLOR = Colors.White;
        private static readonly Brush DEFAULT_BRUSH = Brushes.White;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is Brush)
            {
                var brush = (SolidColorBrush)value;
                return brush.Color;
            }
            return DEFAULT_COLOR;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                var color = (SolidColorBrush)value;
                return color;
            }
            return DEFAULT_BRUSH;
        }
    }
}
