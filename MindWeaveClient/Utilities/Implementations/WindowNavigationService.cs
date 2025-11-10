using Microsoft.Extensions.DependencyInjection;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Linq;
using System.Windows;

namespace MindWeaveClient.Utilities.Implementations
{
    public class WindowNavigationService : IWindowNavigationService
    {
        private readonly IServiceProvider serviceProvider;

        public WindowNavigationService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public void openWindow<TWindow>() where TWindow : Window
        {
            var window = serviceProvider.GetService<TWindow>();
            window.Show();
        }

        public void openDialog<TWindow>(Window owner) where TWindow : Window
        {
            var dialog = serviceProvider.GetService<TWindow>();
            dialog.Owner = owner;
            dialog.ShowDialog();
        }

        public void closeWindowFromContext(object context)
        {
            var window = findWindowByContext(context);
            window?.Close();
        }

        public void closeWindow<T>() where T : Window
        {
            Window windowToClose = Application.Current.Windows.OfType<T>().FirstOrDefault();
            windowToClose?.Close();
        }

        private Window findWindowByContext(object context)
        {
            if (context == null)
            {
                return null;
            }

            return Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.DataContext == context);
        }

    }
}
