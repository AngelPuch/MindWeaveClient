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

        public string email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public string password { get => passwordValue; set { passwordValue = value; OnPropertyChanged(); } }
        public bool showUnverifiedControls { get => showUnverifiedControlsValue; set { showUnverifiedControlsValue = value; OnPropertyChanged(); } }

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
            guestLoginCommand = new RelayCommand((param) => executeGoToGuestJoin());
            resendVerificationCommand = new RelayCommand(async (param) => await executeResendVerificationAsync());
        }

        private bool canExecuteLogin()
        {
            return !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password);
        }

        private async Task executeLoginAsync()
        {
            setBusy(true);
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

                    bool socialConnected = SocialServiceClientManager.instance.connect(result.username);
                    if (!socialConnected)
                    {
                        MessageBox.Show("Could not connect to social features. Friend status and invites might not work.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                    }

                    MatchmakingServiceClientManager.instance.connect();

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
                        showUnverifiedControls = true;
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
                OperationResultDto result = await client.resendVerificationCodeAsync(this.email);
                if (result.success)
                {
                    MessageBox.Show(Lang.InfoMsgBodyCodeSent, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateTo(new VerificationPage(this.email));
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
        public bool isBusy { get => isBusyValue; private set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChangedOnCommands(); } }
        private void setBusy(bool value) { isBusy = value; }
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