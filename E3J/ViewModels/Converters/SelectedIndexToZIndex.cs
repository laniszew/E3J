using System;
using System.Globalization;
using System.Windows.Data;

namespace E3J.ViewModels.Converters
{
    class SelectedIndexToZIndex : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((int)value == 0 || (int)value == 2)
                return 0;
            else
                return 99;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
