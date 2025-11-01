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
        private MatchmakingManagerClient matchmakingClient => MatchmakingServiceClientManager.instance.proxy;

        private string lobbyCodeValue;
        private string guestEmailValue;
        private string desiredUsernameValue;
        private bool isBusyValue;

        public string LobbyCode
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
                raiseCanExecuteChanged();
            }
        }

        public string GuestEmail
        {
            get => guestEmailValue;
            set { guestEmailValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); }
        }

        public string DesiredUsername
        {
            get => desiredUsernameValue;
            set { desiredUsernameValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); }
        }

        public bool IsBusy
        {
            get => isBusyValue;
            private set { setBusy(value); }
        }

        public bool CanJoinAsGuest =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(LobbyCode) && LobbyCode.Length == 6 &&
            Regex.IsMatch(LobbyCode, "^[a-zA-Z0-9]{6}$") &&
            !string.IsNullOrWhiteSpace(GuestEmail) && isValidEmail(GuestEmail) &&
            !string.IsNullOrWhiteSpace(DesiredUsername) && isValidGuestUsername(DesiredUsername);

        public ICommand JoinAsGuestCommand { get; }
        public ICommand GoBackCommand { get; }

        public GuestJoinViewModel(Action<Page> navigateToAction, Action navigateBackAction)
        {
            navigateTo = navigateToAction;
            navigateBack = navigateBackAction;

            JoinAsGuestCommand = new RelayCommand(async param => await executeJoinAsGuestAsync(), param => CanJoinAsGuest);
            GoBackCommand = new RelayCommand(param => navigateBack?.Invoke(), param => !IsBusy);
        }

        private async Task executeJoinAsGuestAsync()
        {
            if (!CanJoinAsGuest) return;

            if (!MatchmakingServiceClientManager.instance.EnsureConnected())
            {
                MessageBox.Show(Lang.CannotConnectMatchmaking, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            setBusy(true);

            var joinRequest = new GuestJoinRequestDto
            {
                lobbyCode = this.LobbyCode,
                guestEmail = this.GuestEmail.Trim(),
                desiredGuestUsername = this.DesiredUsername.Trim()
            };

            try
            {
                GuestJoinResultDto result = await matchmakingClient.joinLobbyAsGuestAsync(joinRequest);

                if (result.success && result.initialLobbyState != null)
                {
                    SessionService.SetSession(result.assignedGuestUsername, null, true);

                    if (!SocialServiceClientManager.instance.Connect(result.assignedGuestUsername))
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
                handleError("An error occurred while connecting to the server", ex);
                MatchmakingServiceClientManager.instance.Disconnect();
            }
            finally
            {
                setBusy(false);
            }
        }

        private void setBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(IsBusy));
            raiseCanExecuteChanged();
        }

        private void raiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(CanJoinAsGuest));
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex?.Message ?? "No details provided.";
            Console.WriteLine($"!!! {message}: {errorDetails}");
            MessageBox.Show($"{message}:\n{errorDetails}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        private bool isValidEmail(string email)
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

        private bool isValidGuestUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            return Regex.IsMatch(username, "^[a-zA-Z0-9]{3,16}$");
        }
    }
}