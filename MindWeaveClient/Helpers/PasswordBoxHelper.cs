using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPassword =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, onBoundPasswordChanged));

        public static string getBoundPassword(DependencyObject d)
        {
            return (string)d.GetValue(BoundPassword);
        }

        public static void setBoundPassword(DependencyObject d, string value)
        {
            d.SetValue(BoundPassword, value);
        }

        private static void onBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is PasswordBox box)) return;
            box.PasswordChanged -= passwordChanged;
            if (!string.IsNullOrEmpty(getBoundPassword(box)))
            {
                box.Password = getBoundPassword(box);
            }
            box.PasswordChanged += passwordChanged;
        }

        private static void passwordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box) setBoundPassword(box, box.Password);
        }
    }
}