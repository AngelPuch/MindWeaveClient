// MindWeaveClient/Helpers/StringToVisibilityConverter.cs
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data; // Necesario para IMultiValueConverter

namespace MindWeaveClient.Helpers // Asegúrate que el namespace sea este
{
    public class StringToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length == 2 && values[0] is string hostUsername && values[1] is string currentPlayerUsername)
            {
                // Compara ignorando mayúsculas/minúsculas
                return (!string.IsNullOrEmpty(hostUsername) && hostUsername.Equals(currentPlayerUsername, StringComparison.OrdinalIgnoreCase))
                    ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}