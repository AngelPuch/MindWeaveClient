// All code must be in English
using System;
// using System.Linq; // <-- Ya no es necesario
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.Helpers
{
    public static class PasswordBoxHelper
    {
        public static readonly DependencyProperty BoundPasswordProperty =
            DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxHelper), new PropertyMetadata(string.Empty, OnBoundPasswordChanged));

        private static bool _isUpdating;

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
                // Remove the handler to prevent infinite loops
                box.PasswordChanged -= HandlePasswordChanged;

                if (!_isUpdating)
                {
                    box.Password = (string)e.NewValue;
                }

                // Add the handler back
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                _isUpdating = true;

                // --- FIX ---
                // Set the bound password property to the actual password,
                // not the reversed one.
                SetBoundPassword(box, box.Password);
                // --- END FIX ---

                _isUpdating = false;
            }
        }
    }
}