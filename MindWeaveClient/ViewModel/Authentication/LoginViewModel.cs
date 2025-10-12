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
        private string _email;
        private string _password;
        private readonly Action<Page> _navigateTo;

        // Propiedades públicas en PascalCase para eliminar advertencias del IDE
        public string email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); OnCanExecuteChanged(LoginCommand); }
        }

        public string password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); OnCanExecuteChanged(LoginCommand); }
        }

        // Comandos públicos en PascalCase
        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand GuestLoginCommand { get; }

        public LoginViewModel(Action<Page> navigateAction)
        {
            _navigateTo = navigateAction;
            LoginCommand = new RelayCommand(async (param) => await ExecuteLoginAsync(), (param) => CanExecuteLogin());
            SignUpCommand = new RelayCommand((param) => ExecuteGoToSignUp());
            ForgotPasswordCommand = new RelayCommand((param) => ExecuteGoToForgotPassword());
            GuestLoginCommand = new RelayCommand((param) => ExecuteGuestLogin());
        }

        private bool CanExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }

        private async Task ExecuteLoginAsync()
        {
            try
            {
                var client = new AuthenticationManagerClient();

                // Creando el DTO con la propiedad 'email' que ahora espera el servidor
                var loginCredentials = new LoginDto
                {
                    email = this.email,
                    password = this.password
                };

                OperationResultDto result = await client.loginAsync(loginCredentials);

                if (result.success)
                {
                    MessageBox.Show(result.message, "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    var currentWindow = Application.Current.MainWindow;
                    var mainAppWindow = new MainWindow();
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

        private void ExecuteGoToSignUp()
        {
            _navigateTo(new CreateAccountPage());
        }

        private void ExecuteGoToForgotPassword()
        {
            MessageBox.Show("Forgot Password functionality not implemented yet.", "Info");
        }

        private void ExecuteGuestLogin()
        {
            MessageBox.Show("Guest Login functionality not implemented yet.", "Info");
        }
    }
}