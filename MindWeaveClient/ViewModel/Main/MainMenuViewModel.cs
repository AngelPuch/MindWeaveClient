using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MindWeaveClient.MatchmakingService;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel, IDisposable
    {
        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly MainMenuValidator validator;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentLobbyService currentLobbyService;

        public string PlayerUsername { get; }

        private string playerAvatarPath;
        public string PlayerAvatarPath
        {
            get => playerAvatarPath;
            set
            {
                playerAvatarPath = value;
                OnPropertyChanged();
            }
        }

        private string joinLobbyCode = string.Empty;
        public string JoinLobbyCode
        {
            get => joinLobbyCode;
            set
            {
                joinLobbyCode = value?.Trim() ?? string.Empty;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(joinLobbyCode))
                {
                    markAsTouched(nameof(JoinLobbyCode));
                }

                validate(validator, this, "JoinLobby");
                OnPropertyChanged(nameof(JoinLobbyCodeError));
            }
        }

        public string JoinLobbyCodeError
        {
            get
            {
                var errors = GetErrors(nameof(JoinLobbyCode)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public ICommand ProfileCommand { get; }
        public ICommand CreateLobbyCommand { get; }
        public ICommand SocialCommand { get; }
        public ICommand SettingsCommand { get; }
        public ICommand JoinLobbyCommand { get; }

        public MainMenuViewModel(
            IMatchmakingService matchmakingService,
            IDialogService dialogService,
            MainMenuValidator validator,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService,
            ICurrentLobbyService currentLobbyService)
        {
            this.matchmakingService = matchmakingService;
            this.dialogService = dialogService;
            this.validator = validator;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;
            this.currentLobbyService = currentLobbyService;

            SessionService.AvatarPathChanged += OnAvatarPathChanged;
            this.matchmakingService.OnLobbyStateUpdated += handleLobbyJoinedSuccess;
            this.matchmakingService.OnLobbyCreationFailed += handleLobbyJoinFailed;


            PlayerUsername = SessionService.Username;
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            ProfileCommand = new RelayCommand(p => 
                this.navigationService.navigateTo<ProfilePage>(), p => !IsBusy);
            CreateLobbyCommand = new RelayCommand(p => 
                this.navigationService.navigateTo<SelectionPuzzlePage>(), p => !IsBusy);
            SocialCommand = new RelayCommand(p => 
                this.navigationService.navigateTo<SocialPage>(), p => !IsBusy);
            SettingsCommand = new RelayCommand(p => 
                this.windowNavigationService.openDialog<SettingsWindow>(Application.Current.MainWindow), p => !IsBusy);
            JoinLobbyCommand = new RelayCommand(async p => 
                await executeJoinLobbyAsync(), p => !HasErrors && !IsBusy);

            validate(validator, this, "JoinLobby");
            this.currentLobbyService = currentLobbyService;
        }

        private void handleLobbyJoinedSuccess(LobbyStateDto state)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (IsBusy)
                {
                    SetBusy(false);
                    currentLobbyService.setInitialState(state);
                    windowNavigationService.openWindow<GameWindow>();
                    windowNavigationService.closeWindow<MainWindow>();
                }
            });
        }

        private void handleLobbyJoinFailed(string reason)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (IsBusy)
                {
                    SetBusy(false);
                    dialogService.showError(reason, Lang.ErrorTitle);
                }
            });
        }

        private void OnAvatarPathChanged(object sender, EventArgs e)
        {
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        private async Task executeJoinLobbyAsync()
        {
            markAsTouched(nameof(JoinLobbyCode));

            if (HasErrors) return;

            SetBusy(true);
            try
            {
                await matchmakingService.joinLobbyAsync(SessionService.Username, JoinLobbyCode);
            }
            catch (EndpointNotFoundException ex)
            {
                SetBusy(false);
                handleError(Lang.ErrorMsgServerOffline, ex);
                matchmakingService.disconnect();
            }
            catch (TimeoutException ex)
            {
                SetBusy(false);
                handleError(Lang.ErrorMsgServerOffline, ex);
                matchmakingService.disconnect();
            }
            catch (Exception ex)
            {
                SetBusy(false);
                handleError(Lang.ErrorMsgGuestJoinFailed, ex);
                matchmakingService.disconnect();
            }
            finally
            {
                SetBusy(false);
            }
        }
        
        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }

        public void Dispose()
        {
            if (matchmakingService != null)
            {
                matchmakingService.OnLobbyStateUpdated -= handleLobbyJoinedSuccess;
                matchmakingService.OnLobbyCreationFailed -= handleLobbyJoinFailed;
            }
            SessionService.AvatarPathChanged -= OnAvatarPathChanged;
        }
    }
}