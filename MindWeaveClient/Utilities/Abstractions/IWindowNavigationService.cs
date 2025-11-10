using System.Windows;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface IWindowNavigationService
    {
        /// <summary>
        /// Opens a new window of the specified type.
        /// </summary>
        /// <typeparam name="TWindow">The type of the Window to open.</typeparam>
        void openWindow<TWindow>() where TWindow : Window;

        /// <summary>
        /// Opens a new window as a modal dialog.
        /// </summary>
        /// <typeparam name="TWindow">The type of the Window to open.</typeparam>
        /// <param name="owner">The owner window for the modal dialog.</param>
        void openDialog<TWindow>(Window owner) where TWindow : Window;

        /// <summary>
        /// Closes the window associated with the provided ViewModel (DataContext).
        /// </summary>
        /// <param name="viewModelContext">The ViewModel whose window should be closed.</param>
        void closeWindowFromContext(object viewModelContext);
        void closeWindow<T>() where T : Window;
    }
}
