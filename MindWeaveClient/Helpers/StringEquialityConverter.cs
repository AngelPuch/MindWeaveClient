using System;
using System.Globalization;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class StringEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            string parameterString = parameter as string;
            return string.Equals(stringValue, parameterString, StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}