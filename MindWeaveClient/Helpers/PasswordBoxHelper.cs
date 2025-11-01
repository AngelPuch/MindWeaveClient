// angelpuch/mindweaveclient/MindWeaveClient-227e8ace26b17f03594f5a0556214f76c93154f8/MindWeaveClient/Helpers/PasswordBoxHelper.cs
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper),
                new PropertyMetadata(string.Empty, onBoundPasswordChanged)); // <-- CORREGIDO a camelCase

        public static string GetBoundPassword(DependencyObject d)
        {
            return (string)d.GetValue(BoundPassword);
        }

        public static void SetBoundPassword(DependencyObject d, string value)
        {
            d.SetValue(BoundPassword, value);
        }

        private static void onBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PasswordBox box)) return;

            box.PasswordChanged -= passwordChanged;

            string newValue = (string)e.NewValue;

            if (box.Password != newValue)
            {
                box.Password = newValue ?? string.Empty;
            }

            box.PasswordChanged += passwordChanged;
        }
        private static void passwordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
 
                SetBoundPassword(box, box.Password);
            }
        }
    }
}