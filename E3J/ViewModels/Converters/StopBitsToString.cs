using System;
using System.Globalization;
using System.IO.Ports;
using System.Windows.Data;

namespace E3J.ViewModels.Converters
{
    /// <summary>
    /// StopBitsToString class
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    [ValueConversion(typeof(StopBits), typeof(string))]
    public class StopBitsToString : IValueConverter
    {
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
            var stopBits = value.ToString();

            if (stopBits == "One")
                return "1";
            if (stopBits == "OnePointFive")
                return "1.5";
            else
                return "2";
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
            var stopBits = value.ToString();

            if (stopBits == "1")
                return StopBits.One;
            if (stopBits == "1.5")
                return StopBits.OnePointFive;
            else
                return StopBits.Two;
        }
    }
}
