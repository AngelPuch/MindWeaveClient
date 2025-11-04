using System.Windows.Controls;

namespace MindWeaveClient.Utilities.Abstractions
{
    public interface INavigationService
    {
        void initialize(Frame frame);
        void navigateTo(string pageKey);
        void goBack();
    }
}