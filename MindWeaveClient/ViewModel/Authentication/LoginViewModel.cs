using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Main;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MindWeaveClient.Services;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private string emailValue;
        private string passwordValue;
        private readonly Action<Page> navigateTo;
        private bool showUnverifiedControlsValue = false;

        public string email
        {
            get => emailValue;
            set { emailValue = value; OnPropertyChanged(); }
        }

        public string password
        {
            get => passwordValue;
            set { passwordValue = value; OnPropertyChanged(); }
        }
        public bool showUnverifiedControls
        {
            get => showUnverifiedControlsValue;
            set { showUnverifiedControlsValue = value; OnPropertyChanged(); }
        }

        // Comandos públicos en PascalCase
        public ICommand loginCommand { get; }
        public ICommand signUpCommand { get; }
        public ICommand forgotPasswordCommand { get; }
        public ICommand guestLoginCommand { get; }
        public ICommand resendVerificationCommand { get; }

        public LoginViewModel(Action<Page> navigateAction)
        {
            navigateTo = navigateAction;
            loginCommand = new RelayCommand(async (param) => await executeLoginAsync(), (param) => canExecuteLogin());
            signUpCommand = new RelayCommand((param) => executeGoToSignUp());
            forgotPasswordCommand = new RelayCommand((param) => executeGoToForgotPassword());
            guestLoginCommand = new RelayCommand((param) => executeGuestLogin());
            resendVerificationCommand = new RelayCommand(async (param) => await executeResendVerificationAsync());
        }

        private bool canExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }

        private async Task executeLoginAsync()
        {
            try
            {
                var client = new AuthenticationManagerClient();
                var loginCredentials = new LoginDto
                {
                    email = this.email,
                    password = this.password
                };

                LoginResultDto result = await client.loginAsync(loginCredentials);

                if (result.operationResult.success)
                {
                    SessionService.setSession(result.username, result.avatarPath);

                    bool socialConnected = SocialServiceClientManager.Instance.Connect(result.username);
                    if (!socialConnected)
                    {
                        // Opcional: Informar al usuario si la conexión social falló,
                        // aunque podría continuar sin ella si es aceptable.
                        MessageBox.Show("Could not connect to social features.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    MessageBox.Show(result.operationResult.message, "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                    var currentWindow = Application.Current.MainWindow;
                    var mainAppWindow = new MainWindow();
                    mainAppWindow.Show();
                    currentWindow?.Close();
                }
                else
                {
                    if (result.resultCode == "ACCOUNT_NOT_VERIFIED")
                    {
                        showUnverifiedControls = true; // Mostramos los controles de reenvío
                    }
                    else
                    {
                        MessageBox.Show(result.operationResult.message, "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while connecting to the server: {ex.Message}", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private async Task executeResendVerificationAsync()
        {
            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.resendVerificationCodeAsync(this.email);

                if (result.success)
                {
                    MessageBox.Show(Properties.Langs.Lang.InfoMsgBodyCodeSent, Properties.Langs.Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateTo(new VerificationPage(this.email));
                }
                else
                {
                    MessageBox.Show(result.message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void executeGoToSignUp()
        {
            navigateTo(new CreateAccountPage());
        }

        private void executeGoToForgotPassword()
        {
            navigateTo(new PasswordRecoveryPage());
        }

        private void executeGuestLogin()
        {
            MessageBox.Show("Guest Login functionality not implemented yet.", "Info");
        }
    }
}