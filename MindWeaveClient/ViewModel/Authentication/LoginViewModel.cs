using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Main;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private string emailValue;
        private string passwordValue;

        // Propiedades en camelCase para el Binding
        public string email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public string password { get => passwordValue; set { passwordValue = value; OnPropertyChanged(); } }

        // Comandos en camelCase para el Binding
        public ICommand loginCommand { get; }
        public ICommand signUpCommand { get; }
        public ICommand forgotPasswordCommand { get; }
        public ICommand guestLoginCommand { get; }

        private readonly Action<Page> navigateTo;

        public LoginViewModel(Action<Page> navigateAction)
        {
            navigateTo = navigateAction;

            loginCommand = new RelayCommand(async (param) => await executeLogin(), (param) => canExecuteLogin());
            signUpCommand = new RelayCommand((param) => executeGoToSignUp());
            forgotPasswordCommand = new RelayCommand((param) => executeGoToForgotPassword());
            guestLoginCommand = new RelayCommand((param) => executeGuestLogin());
        }

        private bool canExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }

        private async Task executeLogin()
        {
            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.loginAsync(this.email, this.password);

                if (result.success)
                {
                    MessageBox.Show(result.message, "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    var currentWindow = Application.Current.MainWindow;
                    var mainAppWindow = new MainWindow(); // Asume que tu ventana principal se llama MainWindow
                    mainAppWindow.Show();
                    currentWindow.Close();
                }
                else
                {
                    MessageBox.Show(result.message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while connecting to the server: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void executeGoToSignUp()
        {
            // Pasamos la acción de navegación a la siguiente página para mantener la funcionalidad
            navigateTo(new CreateAccountPage());
        }

        private void executeGoToForgotPassword()
        {
            MessageBox.Show("Forgot Password functionality not implemented yet.", "Info");
        }

        private void executeGuestLogin()
        {
            MessageBox.Show("Guest Login functionality not implemented yet.", "Info");
        }
    }
}