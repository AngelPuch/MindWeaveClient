using System;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static bool isUpdating;

        public static string GetBoundPassword(DependencyObject d)
        {
            return (string)d.GetValue(BoundPasswordProperty);
        }

        public static void SetBoundPassword(DependencyObject d, string value)
        {
            d.SetValue(BoundPasswordProperty, value);
        }

        private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PasswordBox box)
            {
                box.PasswordChanged -= handlePasswordChanged;

                if (!isUpdating)
                {
                    box.Password = (string)e.NewValue;
                }

                box.PasswordChanged += handlePasswordChanged;
            }
        }

        private static void handlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                isUpdating = true;
                SetBoundPassword(box, box.Password);
                isUpdating = false;
            }
        }
    }
}