using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(page => this.NavigationService?.Navigate(page));
        }

    }
}