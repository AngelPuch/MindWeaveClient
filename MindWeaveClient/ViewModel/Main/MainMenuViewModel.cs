using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Game;
using MindWeaveClient.View.Main;
using MindWeaveClient.View.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
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

                // Marcar como tocado cuando el usuario escribe
                if (!string.IsNullOrEmpty(joinLobbyCode))
                {
                    MarkAsTouched(nameof(JoinLobbyCode));
                }

                Validate(validator, this, "JoinLobby");
                OnPropertyChanged(nameof(JoinLobbyCodeError));
            }
        }

        // Propiedad para obtener el primer error visible del campo
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


            PlayerUsername = SessionService.Username;
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            ProfileCommand = new RelayCommand(p => this.navigationService.navigateTo<ProfilePage>(), p => !IsBusy);
            CreateLobbyCommand = new RelayCommand(p => executeGoToPuzzleSelection(), p => !IsBusy);
            SocialCommand = new RelayCommand(p => this.navigationService.navigateTo<SocialPage>(), p => !IsBusy);
            SettingsCommand = new RelayCommand(p => executeShowSettings(), p => !IsBusy);
            JoinLobbyCommand = new RelayCommand(async p => await executeJoinLobbyAsync(), p => !HasErrors && !IsBusy);

            // Validar inicialmente pero sin marcar como tocado
            Validate(validator, this, "JoinLobby");
        }

        private void OnAvatarPathChanged(object sender, EventArgs e)
        {
            PlayerAvatarPath = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        ~MainMenuViewModel()
        {
            SessionService.AvatarPathChanged -= OnAvatarPathChanged;
        }

        private async Task executeJoinLobbyAsync()
        {
            // Al intentar unirse, marcar el campo como tocado para mostrar errores
            MarkAsTouched(nameof(JoinLobbyCode));

            if (HasErrors) return;

            SetBusy(true);
            try
            {
                await matchmakingService.joinLobbyAsync(SessionService.Username, JoinLobbyCode);
                //currentLobbyService.setInitialState(null);

                windowNavigationService.openWindow<GameWindow>();
                windowNavigationService.closeWindow<MainWindow>();
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

        private void executeShowSettings()
        {
            windowNavigationService.openDialog<SettingsWindow>(Application.Current.MainWindow);
        }

        private void executeGoToPuzzleSelection()
        {
            navigationService.navigateTo<SelectionPuzzlePage>();
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}