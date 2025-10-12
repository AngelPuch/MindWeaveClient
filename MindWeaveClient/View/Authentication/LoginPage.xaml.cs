using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        // El constructor que recibe la acción de navegación es clave
        public LoginPage(Action<Page> navigateAction)
        {
            InitializeComponent();
            this.DataContext = new LoginViewModel(navigateAction);
        }

        // Constructor por defecto, aunque el de arriba es el que deberías usar
        public LoginPage()
        {
            InitializeComponent();
        }
    }
}