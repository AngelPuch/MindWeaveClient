// MindWeaveClient/ViewModel/Authentication/LoginViewModel.cs
using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Main;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MindWeaveClient.Services;
// *** NO necesitamos 'using MindWeaveClient;' aquí ahora ***
using MindWeaveClient.Properties.Langs; // Para Lang

namespace MindWeaveClient.ViewModel.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        // --- Propiedades (sin cambios) ---
        private string emailValue;
        private string passwordValue;
        private readonly Action<Page> navigateTo;
        private bool showUnverifiedControlsValue = false;

        public string email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public string password { get => passwordValue; set { passwordValue = value; OnPropertyChanged(); } }
        public bool showUnverifiedControls { get => showUnverifiedControlsValue; set { showUnverifiedControlsValue = value; OnPropertyChanged(); } }

        // --- Comandos (sin cambios) ---
        public ICommand loginCommand { get; }
        public ICommand signUpCommand { get; }
        public ICommand forgotPasswordCommand { get; }
        public ICommand guestLoginCommand { get; }
        public ICommand resendVerificationCommand { get; }

        // --- Constructor (sin cambios) ---
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
            SetBusy(true); // Usar helper
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
                    SessionService.setSession(result.username, result.avatarPath); // Guardar sesión

                    // *** CONECTAR SERVICIO SOCIAL (necesario para estado online) ***
                    bool socialConnected = SocialServiceClientManager.Instance.Connect(result.username);
                    if (!socialConnected)
                    {
                        MessageBox.Show("Could not connect to social features. Friend status and invites might not work.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning); // TODO: Lang
                    }
                    // *** YA NO SUSCRIBIMOS A INVITACIONES AQUÍ ***
                    // else { App.SubscribeToGlobalInvites(); }

                    // Conectar Matchmaking (opcional aquí, podría hacerse al entrar a lobby/selección puzzle)
                    MatchmakingServiceClientManager.Instance.Connect();

                    MessageBox.Show(result.operationResult.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);

                    // Navegación a MainWindow (sin cambios)
                    var currentWindow = Application.Current.MainWindow; // Obtiene AuthenticationWindow
                    var mainAppWindow = new MainWindow(); // Crea la nueva ventana principal
                    mainAppWindow.Show(); // Muestra la ventana principal
                    currentWindow?.Close(); // Cierra la ventana de autenticación
                }
                else
                {
                    // Manejo de errores de login (sin cambios)
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
                HandleError("An error occurred while connecting to the server", ex); // Usar helper
            }
            finally
            {
                SetBusy(false); // Usar helper
            }
        }

        // --- Otros métodos execute* (sin cambios) ---
        private async Task executeResendVerificationAsync()
        {
            SetBusy(true);
            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.resendVerificationCodeAsync(this.email);
                if (result.success)
                {
                    MessageBox.Show(Lang.InfoMsgBodyCodeSent, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateTo(new VerificationPage(this.email));
                }
                else { HandleError(result.message, null); }
            }
            catch (Exception ex) { HandleError("Error resending code", ex); }
            finally { SetBusy(false); }
        }
        private void executeGoToSignUp() { navigateTo(new CreateAccountPage()); }
        private void executeGoToForgotPassword() { navigateTo(new PasswordRecoveryPage()); }
        private void executeGuestLogin() { MessageBox.Show("Guest Login functionality not implemented yet.", "Info"); } // TODO: Implementar

        // --- Helpers ---
        private bool isBusyValue = false;
        public bool IsBusy { get => isBusyValue; private set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChangedOnCommands(); } }
        private void SetBusy(bool value) { IsBusy = value; }
        private void HandleError(string message, Exception ex)
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