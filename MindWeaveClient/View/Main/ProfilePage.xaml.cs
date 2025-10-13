using MindWeaveClient.ViewModel.Main;
using System.Windows.Controls;

namespace MindWeaveClient.View.Main
{
    /// <summary>
    /// Lógica de interacción para ProfilePage.xaml
    /// </summary>
    public partial class ProfilePage : Page
    {
        public ProfilePage()
        {
            InitializeComponent();
            DataContext = new ProfileViewModel(() => {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            });
        }
    }
}
