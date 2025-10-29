using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class GuestJoinViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateTo;
        private readonly Action navigateBack;
        private MatchmakingManagerClient matchmakingClient => MatchmakingServiceClientManager.Instance.Proxy;

        private string lobbyCodeValue;
        private string guestEmailValue;
        private string desiredUsernameValue;
        private bool isBusyValue;

        public string lobbyCode
        {
            get => lobbyCodeValue;
            set
            {
                if (value != null && !Regex.IsMatch(value, "^[a-zA-Z0-9]*$"))
                {
                    value = Regex.Match(lobbyCodeValue ?? "", "^[a-zA-Z0-9]*").Value;
                }
                lobbyCodeValue = value?.Trim();
                OnPropertyChanged();
                RaiseCanExecuteChanged();
            }
        }

        public string guestEmail
        {
            get => guestEmailValue;
            set { guestEmailValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); }
        }

        public string desiredUsername
        {
            get => desiredUsernameValue;
            set { desiredUsernameValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); }
        }

        public bool isBusy
        {
            get => isBusyValue;
            private set { SetBusy(value); }
        }

        public bool canJoinAsGuest =>
            !isBusy &&
            !string.IsNullOrWhiteSpace(lobbyCode) && lobbyCode.Length == 6 &&
            Regex.IsMatch(lobbyCode, "^[a-zA-Z0-9]{6}$") &&
            !string.IsNullOrWhiteSpace(guestEmail) && IsValidEmail(guestEmail) &&
            !string.IsNullOrWhiteSpace(desiredUsername) && IsValidGuestUsername(desiredUsername);

        public ICommand joinAsGuestCommand { get; }
        public ICommand goBackCommand { get; }

        public GuestJoinViewModel(Action<Page> navigateToAction, Action navigateBackAction)
        {
            navigateTo = navigateToAction;
            navigateBack = navigateBackAction;

            joinAsGuestCommand = new RelayCommand(async param => await executeJoinAsGuestAsync(), param => canJoinAsGuest);
            goBackCommand = new RelayCommand(param => navigateBack?.Invoke(), param => !isBusy);
        }

        private async Task executeJoinAsGuestAsync()
        {
            if (!canJoinAsGuest) return;

            if (!MatchmakingServiceClientManager.Instance.EnsureConnected())
            {
                MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetBusy(true);

            var joinRequest = new GuestJoinRequestDto
            {
                lobbyCode = this.lobbyCode,
                guestEmail = this.guestEmail.Trim(),
                desiredGuestUsername = this.desiredUsername.Trim()
            };

            try
            {
                GuestJoinResultDto result = await matchmakingClient.joinLobbyAsGuestAsync(joinRequest);

                if (result.success && result.initialLobbyState != null)
                {
                    SessionService.setSession(result.assignedGuestUsername, null, true);

                    if (!SocialServiceClientManager.Instance.Connect(result.assignedGuestUsername))
                    {
                        MessageBox.Show("Could not connect to social features. Chat might not work correctly.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }

                    MessageBox.Show(result.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);

                    var currentWindow = Application.Current.Windows.OfType<AuthenticationWindow>().FirstOrDefault();
                    var mainAppWindow = new MainWindow();

                    var lobbyPage = new LobbyPage();
                    lobbyPage.DataContext = new LobbyViewModel(
                        result.initialLobbyState,
                        page => mainAppWindow.MainFrame.Navigate(page),
                        () => mainAppWindow.MainFrame.Navigate(new MainMenuPage(page => mainAppWindow.MainFrame.Navigate(page))) 
                    );
                    mainAppWindow.MainFrame.Navigate(lobbyPage);

                    mainAppWindow.Show();
                    currentWindow?.Close();
                }
                else
                {
                    MessageBox.Show(result.message ?? "Unexpected error", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                HandleError("An error occurred while connecting to the server", ex);
                MatchmakingServiceClientManager.Instance.Disconnect();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(isBusy));
            RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(canJoinAsGuest));
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void HandleError(string message, Exception ex)
        {
            string errorDetails = ex?.Message ?? "No details provided.";
            Console.WriteLine($"!!! {message}: {errorDetails}");
            MessageBox.Show($"{message}:\n{errorDetails}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidGuestUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            return Regex.IsMatch(username, "^[a-zA-Z0-9]{3,16}$");
        }
    }
}