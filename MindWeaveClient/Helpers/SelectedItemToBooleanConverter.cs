using System;
using System.Globalization;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class SelectedItemToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked && isChecked)
            {
                return Binding.DoNothing;
            }
            return Binding.DoNothing;
        }
    }
}