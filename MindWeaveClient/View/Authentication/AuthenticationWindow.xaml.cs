using MindWeaveClient.Utilities.Abstractions;
using System.Windows;

namespace MindWeaveClient.View.Authentication
{
    public partial class AuthenticationWindow : Window
    {
        private readonly INavigationService navigationService;

        public AuthenticationWindow(INavigationService navigationService, LoginPage startPage)
        {
            InitializeComponent();

            this.navigationService = navigationService;
            this.navigationService.initialize(AuthenticationFrame);

            AuthenticationFrame.Navigate(startPage);
        }

    }
}