using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace E3J.ViewModels.Converters
{
    /// <summary>
    /// SolidColorToBrush class
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    [ValueConversion(typeof(Color), typeof(Brush))]
    public class SolidColorToBrush : IValueConverter
    {
        /// <summary>
        /// The default color
        /// </summary>
        private static readonly Color DEFAULT_COLOR = Colors.White;
        /// <summary>
        /// The default brush
        /// </summary>
        private static readonly Brush DEFAULT_BRUSH = Brushes.White;

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                var color = (SolidColorBrush)value;
                var colorWithoutAlpha = color.ToString().Remove(1, 2);
                return colorWithoutAlpha;
            }
            return DEFAULT_BRUSH;
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Brush)
            {
                var brush = (SolidColorBrush)value;
                return brush.Color;
            }
            return DEFAULT_COLOR;
        }
    }
}
