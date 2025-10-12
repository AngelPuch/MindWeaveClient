using MindWeaveClient.ViewModel.Authentication;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    /// <summary>
    /// Lógica de interacción para CreateAccountPage.xaml
    /// </summary>
    public partial class CreateAccountPage : Page
    {
        public CreateAccountPage()
        {
            InitializeComponent(); // Agrega esta línea
            this.DataContext = new CreateAccountViewModel(page => this.NavigationService?.Navigate(page));
        }
    }
}