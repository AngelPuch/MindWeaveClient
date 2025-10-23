using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class KickButtonVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0] = isHost (bool)
            // values[1] = playerUsername (string)
            // values[2] = hostUsername (string)
            if (values.Length == 3 && values[0] is bool isHost && values[1] is string playerUsername && values[2] is string hostUsername)
            {
                // Visible si soy host Y el jugador de esta fila NO soy yo
                if (isHost && !string.IsNullOrEmpty(playerUsername) && !playerUsername.Equals(hostUsername, StringComparison.OrdinalIgnoreCase))
                {
                    return Visibility.Visible;
                }
            }
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}