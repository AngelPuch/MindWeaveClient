using MindWeaveClient.ViewModel.Authentication;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        public LoginPage(LoginViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}