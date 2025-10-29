using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Services;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.View.Settings;
using MindWeaveClient.ViewModel.Game;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateTo;
        private readonly Page mainMenuPage;

        public string playerUsername { get; }
        public string playerAvatarPath { get; }

        public ICommand profileCommand { get; }
        public ICommand createLobbyCommand { get; } 
        public ICommand socialCommand { get; }
        public ICommand settingsCommand { get; }
        public ICommand joinLobbyCommand { get; } 

        private string joinLobbyCodeValue = string.Empty;
        public string joinLobbyCode
        {
            get => joinLobbyCodeValue;
            set
            {
                joinLobbyCodeValue = value?.ToUpper().Trim() ?? string.Empty;
                if (joinLobbyCodeValue.Length > 6) { joinLobbyCodeValue = joinLobbyCodeValue.Substring(0, 6); }
                OnPropertyChanged();
                OnPropertyChanged(nameof(canJoinLobby));
                ((RelayCommand)joinLobbyCommand).RaiseCanExecuteChanged();
            }
        }
        public bool canJoinLobby => !isBusy && !string.IsNullOrWhiteSpace(joinLobbyCode) && joinLobbyCode.Length == 6;

        private bool isBusyValue;
        public bool isBusy
        {
            get => isBusyValue;
            set
            {
                isBusyValue = value;
                OnPropertyChanged();
                ((RelayCommand)createLobbyCommand).RaiseCanExecuteChanged(); 
                ((RelayCommand)joinLobbyCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(canJoinLobby));
            }
        }

        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.Instance.Proxy;

        public MainMenuViewModel(Action<Page> navigateTo, Page mainMenuPage)
        {
            this.navigateTo = navigateTo;
            this.mainMenuPage = mainMenuPage;

            playerUsername = SessionService.username;
            playerAvatarPath = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            profileCommand = new RelayCommand(p => executeGoToProfile());
            createLobbyCommand = new RelayCommand(p => executeGoToPuzzleSelection(), p => !isBusy);
            socialCommand = new RelayCommand(p => executeGoToSocial());
            settingsCommand = new RelayCommand(p => executeShowSettings(), p => !isBusy);
            joinLobbyCommand = new RelayCommand(async p => await executeJoinLobbyAsync(), p => canJoinLobby);

            Console.WriteLine($"MainMenuViewModel Initialized. Avatar Path: {playerAvatarPath}");
        }
        private void executeGoToPuzzleSelection()
        {
            var selectionPage = new SelectionPuzzlePage(); 
            selectionPage.DataContext = new SelectionPuzzleViewModel(
                navigateTo,         
                () => navigateTo(mainMenuPage)
            );
            navigateTo(selectionPage);
        }
        private async Task executeJoinLobbyAsync()
        {
            if (!canJoinLobby) return;
            if (!Regex.IsMatch(joinLobbyCode, "^[A-Z0-9]{6}$")) { /*...*/ return; }

            isBusy = true;
            try
            {
                if (!MatchmakingServiceClientManager.Instance.EnsureConnected()) { /*...*/ return; }

                matchmakingProxy.joinLobby(SessionService.username, joinLobbyCode);
                MessageBox.Show($"Attempting to join lobby {joinLobbyCode}...", "Joining", MessageBoxButton.OK, MessageBoxImage.Information); 

                var lobbyPage = new LobbyPage();
                lobbyPage.DataContext = new LobbyViewModel(
                    null, 
                    navigateTo,
                    () => navigateTo(mainMenuPage)
                );
                navigateTo(lobbyPage);
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"An error occurred while joining lobby: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); // TODO: Lang
                MatchmakingServiceClientManager.Instance.Disconnect();
            }
            finally
            {
                isBusy = false;
            }
        }
        

        private void executeGoToProfile()
        {
            var profilePage = new ProfilePage();
            profilePage.DataContext = new ProfileViewModel(
                () => navigateTo(mainMenuPage),  
                () => executeGoToEditProfile() 
            );
            navigateTo(profilePage);
        }

        private void executeGoToSocial() {
            navigateTo(new SocialPage());
        }

        private void executeGoToEditProfile()
        {
            var editProfilePage = new EditProfilePage();
            editProfilePage.DataContext = new EditProfileViewModel(
                () => executeGoToProfile(), 
                () => executeGoToSelectAvatar()
            );
            navigateTo(editProfilePage);
        }

        private void executeGoToSelectAvatar()
        {
            var selectAvatarPage = new SelectAvatarPage();
            selectAvatarPage.DataContext = new SelectAvatarViewModel(
                () => executeGoToEditProfile()
            );
            navigateTo(selectAvatarPage);
        }
        private void executeShowSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = Application.Current.MainWindow;
            bool? result = settingsWindow.ShowDialog();

            if (result == true)
            {
                Console.WriteLine("Settings saved.");
            }
            else
            {
                // Settings were canceled
                Console.WriteLine("Settings canceled.");
            }
        }

    }
}