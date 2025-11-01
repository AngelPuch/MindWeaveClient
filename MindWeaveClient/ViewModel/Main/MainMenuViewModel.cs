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

        public string PlayerUsername { get; }
        public string PlayerAvatarPath { get; }

        public ICommand ProfileCommand { get; }
        public ICommand CreateLobbyCommand { get; }
        public ICommand SocialCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand JoinLobbyCommand { get; } 

        private string joinLobbyCodeValue = string.Empty;
        public string JoinLobbyCode
        {
            get => joinLobbyCodeValue;
            set
            {
                joinLobbyCodeValue = value?.ToUpper().Trim() ?? string.Empty;
                if (joinLobbyCodeValue.Length > 6) { joinLobbyCodeValue = joinLobbyCodeValue.Substring(0, 6); }
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanJoinLobby));
                ((RelayCommand)JoinLobbyCommand).RaiseCanExecuteChanged();
            }
        }
        public bool CanJoinLobby => !IsBusy && !string.IsNullOrWhiteSpace(JoinLobbyCode) && JoinLobbyCode.Length == 6;

        private bool isBusyValue;
        public bool IsBusy
        {
            get => isBusyValue;
            set
            {
                isBusyValue = value;
                OnPropertyChanged();
                ((RelayCommand)CreateLobbyCommand).RaiseCanExecuteChanged(); 
                ((RelayCommand)JoinLobbyCommand).RaiseCanExecuteChanged();
                OnPropertyChanged(nameof(CanJoinLobby));
            }
        }

        private MatchmakingManagerClient matchmakingProxy => MatchmakingServiceClientManager.instance.proxy;

        public MainMenuViewModel(Action<Page> navigateTo, Page mainMenuPage)
        {
            this.navigateTo = navigateTo;
            this.mainMenuPage = mainMenuPage;

            PlayerUsername = SessionService.Username;
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            ProfileCommand = new RelayCommand(p => executeGoToProfile());
            CreateLobbyCommand = new RelayCommand(p => executeGoToPuzzleSelection(), p => !IsBusy);
            SocialCommand = new RelayCommand(p => executeGoToSocial());
            SettingsCommand = new RelayCommand(p => executeShowSettings(), p => !IsBusy);
            JoinLobbyCommand = new RelayCommand(async p => await executeJoinLobbyAsync(), p => CanJoinLobby);

            Console.WriteLine($"MainMenuViewModel Initialized. Avatar Path: {PlayerAvatarPath}");
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
            if (!CanJoinLobby) return;
            if (!Regex.IsMatch(JoinLobbyCode, "^[A-Z0-9]{6}$")) { /*...*/ return; }

            IsBusy = true;
            try
            {
                if (!MatchmakingServiceClientManager.instance.EnsureConnected()) { /*...*/ return; }

                matchmakingProxy.joinLobby(SessionService.Username, JoinLobbyCode);
                MessageBox.Show($"Attempting to join lobby {JoinLobbyCode}...", "Joining", MessageBoxButton.OK, MessageBoxImage.Information); 

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
                MatchmakingServiceClientManager.instance.Disconnect();
            }
            finally
            {
                IsBusy = false;
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
                 Console.WriteLine("Settings canceled.");
            }
        }

    }
}