using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        // Corregido: de "getBoundPassword" a "GetBoundPassword"
        public static string GetBoundPassword(DependencyObject d)
        {
            return (string)d.GetValue(BoundPassword);
        }

        // Corregido: de "setBoundPassword" a "SetBoundPassword"
        public static void SetBoundPassword(DependencyObject d, string value)
        {
            d.SetValue(BoundPassword, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PasswordBox box)) return;
            box.PasswordChanged -= PasswordChanged;

            if (!string.IsNullOrEmpty(GetBoundPassword(box)))
            {
                box.Password = GetBoundPassword(box);
            }

            box.PasswordChanged += PasswordChanged;
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                SetBoundPassword(box, box.Password);
            }
        }
    }
}