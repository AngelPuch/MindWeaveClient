using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class EqualityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool areEqual = object.Equals(value?.ToString(), parameter?.ToString());
            return areEqual ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}