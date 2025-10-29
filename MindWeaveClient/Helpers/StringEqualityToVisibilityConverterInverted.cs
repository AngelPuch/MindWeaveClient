using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class StringEqualityToVisibilityConverterInverted : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string stringValue = value as string;
            string parameterString = parameter as string;
            bool areEqual = string.Equals(stringValue, parameterString, StringComparison.OrdinalIgnoreCase);
            return areEqual ? Visibility.Collapsed : Visibility.Visible; // Inverted logic
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}