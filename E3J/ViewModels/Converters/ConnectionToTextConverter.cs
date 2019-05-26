using System;
using System.Globalization;
using System.Windows.Data;

namespace E3J.ViewModels.Converters
{
    /// <summary>
    /// ConnectionToTextConverter class
    /// </summary>
    /// <seealso cref="System.Windows.Data.IValueConverter" />
    public class ConnectionToTextConverter : IValueConverter
    {
        /// <summary>
        /// The connected
        /// </summary>
        private const string CONNECTED = "Connected";
        /// <summary>
        /// The disconnected
        /// </summary>
        private const string DISCONNECTED = "Disconnected";

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
            var connection = DISCONNECTED;
            if (value is bool)
            {
                var val = (bool) value;
                connection = val ? CONNECTED : DISCONNECTED;
            }
            return connection;
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
        /// <exception cref="System.NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
