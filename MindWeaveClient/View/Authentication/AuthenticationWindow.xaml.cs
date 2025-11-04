using System;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class AuthenticationWindow : Window
    {
        public AuthenticationWindow()
        {
            InitializeComponent();
            Action<Page> navigateAction = (pageToNavigate) =>
            {
                AuthenticationFrame.Navigate(pageToNavigate);
            };

            LoginPage loginPage = new LoginPage(navigateAction);

            AuthenticationFrame.Navigate(loginPage);
        }

    }
}