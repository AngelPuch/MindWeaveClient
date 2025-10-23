using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media; // Necesario para Brush y Colors

namespace MindWeaveClient.Helpers
{
    public class BoolToOnlineColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convierte bool (IsOnline) a color
            if (value is bool isOnline && isOnline)
            {
                return Brushes.LimeGreen; // Verde si está online
            }
            return Brushes.Gray; // Gris si está offline o valor no es bool
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // No necesario para OneWay binding
        }
    }
}