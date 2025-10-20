using MindWeaveClient.ViewModel.Main; // Asegúrate que apunte a donde está tu clase Avatar
using System;
using System.Globalization;
using System.Windows.Data;

namespace MindWeaveClient.Helpers
{
    public class SelectedItemToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // value es el DataContext actual (un Avatar)
            // parameter es el Avatar seleccionado en el ViewModel
            return value != null && value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Si IsChecked es true, devolvemos el DataContext actual (el Avatar)
            // para que se asigne a selectedAvatar en el ViewModel
            if (value is bool isChecked && isChecked)
            {
                // Necesitamos una forma de obtener el DataContext original aquí.
                // Esto es complicado con IValueConverter simple.
                // Es mejor usar MultiBinding o manejar la selección en el code-behind o ViewModel.

                // Por simplicidad ahora, haremos el ConvertBack no funcional para TwoWay binding directo.
                // La selección se manejará por el evento GotFocus que ya tienes.
                return Binding.DoNothing;
            }
            return Binding.DoNothing;
        }
    }
}