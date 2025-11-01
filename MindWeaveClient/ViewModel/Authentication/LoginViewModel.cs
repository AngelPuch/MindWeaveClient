using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Main;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MindWeaveClient.Services;
using MindWeaveClient.Properties.Langs;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private string emailValue;
        private string passwordValue;
        private readonly Action<Page> navigateTo;
        private bool showUnverifiedControlsValue = false;

        public string Email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public string Password { get => passwordValue; set { passwordValue = value; OnPropertyChanged(); } }
        public bool ShowUnverifiedControls { get => showUnverifiedControlsValue; set { showUnverifiedControlsValue = value; OnPropertyChanged(); } }

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand GuestLoginCommand { get; }
        public ICommand ResendVerificationCommand { get; }

        public LoginViewModel(Action<Page> navigateAction)
        {
            navigateTo = navigateAction;
            LoginCommand = new RelayCommand(async (param) => await executeLoginAsync(), (param) => canExecuteLogin());
            SignUpCommand = new RelayCommand((param) => executeGoToSignUp());
            ForgotPasswordCommand = new RelayCommand((param) => executeGoToForgotPassword());
            GuestLoginCommand = new RelayCommand((param) => executeGoToGuestJoin());
            ResendVerificationCommand = new RelayCommand(async (param) => await executeResendVerificationAsync());
        }

        private bool canExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Password);
        }

        private async Task executeLoginAsync()
        {
            setBusy(true);
            try
            {
                var client = new AuthenticationManagerClient();
                var loginCredentials = new LoginDto
                {
                    email = this.Email,
                    password = this.Password
                };

                LoginResultDto result = await client.loginAsync(loginCredentials);

                if (result.operationResult.success)
                {
                    SessionService.SetSession(result.username, result.avatarPath);

                    bool socialConnected = SocialServiceClientManager.instance.Connect(result.username);
                    if (!socialConnected)
                    {
                        MessageBox.Show("Could not connect to social features. Friend status and invites might not work.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                    }

                    MatchmakingServiceClientManager.instance.Connect();

                    MessageBox.Show(result.operationResult.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);

                    var currentWindow = Application.Current.MainWindow;
                    var mainAppWindow = new MainWindow();
                    mainAppWindow.Show();
                    currentWindow?.Close();
                }
                else
                {
                    if (result.resultCode == "ACCOUNT_NOT_VERIFIED")
                    {
                        ShowUnverifiedControls = true;
                    }
                    else
                    {
                        MessageBox.Show(result.operationResult.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                handleError("An error occurred while connecting to the server", ex);
            }
            finally
            {
                setBusy(false);
            }
        }

        private async Task executeResendVerificationAsync()
        {
            setBusy(true);
            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.resendVerificationCodeAsync(this.Email);
                if (result.success)
                {
                    MessageBox.Show(Lang.InfoMsgBodyCodeSent, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateTo(new VerificationPage(this.Email));
                }
                else { handleError(result.message, null); }
            }
            catch (Exception ex) { handleError("Error resending code", ex); }
            finally { setBusy(false); }
        }
        private void executeGoToSignUp() { navigateTo(new CreateAccountPage()); }
        private void executeGoToForgotPassword() { navigateTo(new PasswordRecoveryPage()); }
        private void executeGoToGuestJoin()
        {
            navigateTo(new GuestJoinPage());
        }

        private bool isBusyValue;
        public bool IsBusy { get => isBusyValue; private set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChangedOnCommands(); } }
        private void setBusy(bool value) { IsBusy = value; }
        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : "No details";
            Console.WriteLine($"!!! {message}: {errorDetails}");
            MessageBox.Show($"{message}: {errorDetails}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private void RaiseCanExecuteChangedOnCommands()
        {
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }
    }
}