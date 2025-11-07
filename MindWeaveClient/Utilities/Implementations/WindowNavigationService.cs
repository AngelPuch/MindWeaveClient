using Microsoft.Extensions.DependencyInjection;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

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

        public void closeWindowFromContext(object viewModelContext)
        {
            if (viewModelContext == null)
            {
                return;
            }

            var window = Application.Current.Windows.OfType<Window>()
                .FirstOrDefault(w => w.DataContext == viewModelContext);

            if (window == null)
            {
                window = Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w.FindName("AuthenticationFrame") != null ||
                                         w.FindName("MainFrame") != null);

                if (window != null && window.Content is Frame frame)
                {
                    if (frame.Content is Page page && page.DataContext == viewModelContext)
                    {
                        window.Close();
                        return;
                    }
                }

                window = Application.Current.Windows.OfType<Window>()
                    .FirstOrDefault(w => w.IsActive && w.DataContext != null);

                if (window != null && window.DataContext == viewModelContext)
                {
                    window.Close();
                }
            }

            window?.Close();
        }
    }
}
