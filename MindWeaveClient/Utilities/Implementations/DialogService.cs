using System.Windows;
using MindWeaveClient.Utilities.Abstractions;

namespace MindWeaveClient.Utilities.Implementations
{
    public class DialogService : IDialogService
    {
        public void showInfo(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void showError(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public bool showWarning(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
        }
    }
}
