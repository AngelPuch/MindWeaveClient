using System.Windows;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface IWindowNavigationService
    {
        void openWindow<TWindow>() where TWindow : Window;

        void openDialog<TWindow>(Window owner) where TWindow : Window;

        void closeWindow<T>() where T : Window;
    }
}
