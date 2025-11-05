using System;
using MindWeaveClient.ViewModel.Authentication;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.View.Authentication
{
    public partial class PasswordRecoveryPage : Page
    {
        public PasswordRecoveryPage(Action<Page> navigateAction)
        {
            InitializeComponent();

            Action navigateToLogin = () =>
            {
                navigateAction(new LoginPage(navigateAction));
            };

            DataContext = new PasswordRecoveryViewModel(navigateToLogin, navigateToLogin);
        }

        private void CodeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}