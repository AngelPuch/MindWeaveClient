using System.Windows.Controls;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface INavigationService
    {
        /// <summary>
        /// Sets the navigation frame to be controlled by this service.
        /// </summary>
        void initialize(Frame navigationFrame);

        /// <summary>
        /// Navigates to a new page using its type.
        /// </summary>
        void navigateTo<TView>() where TView : Page;

        /// <summary>
        /// Navigates to the previous page in the journal.
        /// </summary>
        void goBack();
    }
}