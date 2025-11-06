using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.View.Settings;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> navigateAction;
        private readonly Page mainMenuPage;
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly MainMenuValidator validator;

        public string PlayerUsername { get; }
        public string PlayerAvatarPath { get; }

        private string joinLobbyCode = string.Empty;
        public string JoinLobbyCode
        {
            get => joinLobbyCode;
            set
            {
                joinLobbyCode = value?.Trim() ?? string.Empty;
                OnPropertyChanged();
                Validate(validator, this, "JoinLobby");
            }
        }

        public ICommand ProfileCommand { get; }
        public ICommand CreateLobbyCommand { get; }
        public ICommand SocialCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand JoinLobbyCommand { get; }

        public MainMenuViewModel(Action<Page> navigateAction, Page mainMenuPage)
        {
            this.navigateAction = navigateAction;
            this.mainMenuPage = mainMenuPage;

            this.matchmakingService = new Services.Implementations.MatchmakingService();
            this.dialogService = new DialogService();
            this.validator = new MainMenuValidator();

            PlayerUsername = SessionService.Username;
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            ProfileCommand = new RelayCommand(p => executeGoToProfile(), p => !IsBusy);
            CreateLobbyCommand = new RelayCommand(p => executeGoToPuzzleSelection(), p => !IsBusy);
            SocialCommand = new RelayCommand(p => executeGoToSocial(), p => !IsBusy);
            SettingsCommand = new RelayCommand(p => executeShowSettings(), p => !IsBusy);
            JoinLobbyCommand = new RelayCommand(async p => await executeJoinLobbyAsync(), p => !HasErrors && !IsBusy);

            Validate(validator, this, "JoinLobby");
        }

        private async Task executeJoinLobbyAsync()
        {
            if (HasErrors) return;

            SetBusy(true);
            try
            {
                await matchmakingService.joinLobbyAsync(SessionService.Username, JoinLobbyCode);
                
                var lobbyPage = new LobbyPage(null, navigateAction, () => navigateAction(mainMenuPage));
                navigateAction(lobbyPage);
            }
            catch (InvalidOperationException ex)
            {
                dialogService.showError(ex.Message, Lang.ErrorTitle);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                MatchmakingServiceClientManager.instance.Disconnect();
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgGuestJoinFailed, ex);
                MatchmakingServiceClientManager.instance.Disconnect();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private static void executeShowSettings()
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.Owner = Application.Current.MainWindow;
            settingsWindow.ShowDialog();
        }

        private void executeGoToPuzzleSelection()
        {
            var selectionPage = new SelectionPuzzlePage();
            selectionPage.DataContext = new SelectionPuzzleViewModel(
                navigateAction,
                () => navigateAction(mainMenuPage)
            );
            navigateAction(selectionPage);
        }

        private void executeGoToProfile()
        {
            var profilePage = new ProfilePage();
            profilePage.DataContext = new ProfileViewModel(
                () => navigateAction(mainMenuPage),
                () => executeGoToEditProfile()
            );
            navigateAction(profilePage);
        }

        private void executeGoToSocial()
        {
            navigateAction(new SocialPage());
        }

        private void executeGoToEditProfile()
        {
            var editProfilePage = new EditProfilePage();
            editProfilePage.DataContext = new EditProfileViewModel(
                () => executeGoToProfile(),
                () => executeGoToSelectAvatar()
            );
            navigateAction(editProfilePage);
        }

        private void executeGoToSelectAvatar()
        {
            var selectAvatarPage = new SelectAvatarPage();
            selectAvatarPage.DataContext = new SelectAvatarViewModel(
                () => executeGoToEditProfile()
            );
            navigateAction(selectAvatarPage);
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}