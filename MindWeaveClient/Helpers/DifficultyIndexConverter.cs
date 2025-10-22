// MindWeaveClient/Helpers/DifficultyIndexConverter.cs
using System;
using System.Globalization;
using System.Windows.Data; // Necesario para IValueConverter

namespace MindWeaveClient.Helpers // Asegúrate que el namespace sea este
{
    public class DifficultyIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convierte ID (1, 2, 3) a Index (0, 1, 2)
            if (value is int id && id > 0) return id - 1;
            // Si el valor no es un int válido o es <= 0, devuelve 0 (índice de 'Easy')
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Convierte Index (0, 1, 2) a ID (1, 2, 3)
            if (value is int index && index >= 0) return index + 1;
            // Si el índice es inválido, devuelve 1 (ID de 'Easy')
            return 1;
        }
    }
}