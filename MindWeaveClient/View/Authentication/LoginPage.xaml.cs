using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        // El constructor que recibe la acción de navegación es clave
        public LoginPage()
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(page => this.NavigationService?.Navigate(page));
        }

    }
}