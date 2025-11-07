using MindWeaveClient.ViewModel.Authentication;
using MindWeaveClient.ViewModel.Game;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        public LoginPage(LobbyViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}