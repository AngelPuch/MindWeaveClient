using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Authentication;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MindWeaveClient.View.Authentication
{
    public partial class LoginPage : Page
    {
        public LoginPage(Action<Page> navigateAction)
        {
            InitializeComponent();
            var viewModel = new LoginViewModel(navigateAction);
            viewModel.LoginSuccess += OnLoginSuccess;

            DataContext = viewModel;
        }

        private void OnLoginSuccess(object sender, EventArgs e)
        {
            var currentWindow = Window.GetWindow(this);

            var mainAppWindow = new MainWindow();
            mainAppWindow.Show();

            currentWindow?.Close();
        }

    }
}