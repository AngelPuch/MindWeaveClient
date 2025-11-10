using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Authentication;
using MindWeaveClient.View.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class GuestJoinViewModel : BaseViewModel
    {
        private string lobbyCode;
        private string guestEmail;
        private string desiredUsername;

        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly GuestJoinValidator validator;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;
        private readonly ICurrentLobbyService currentLobbyService;

        public string LobbyCode
        {
            get => lobbyCode;
            set
            {
                lobbyCode = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(LobbyCode));
                }
                Validate(validator, this);
                OnPropertyChanged(nameof(LobbyCodeError));
            }
        }

        public string GuestEmail
        {
            get => guestEmail;
            set
            {
                guestEmail = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(GuestEmail));
                }
                Validate(validator, this);
                OnPropertyChanged(nameof(GuestEmailError));
            }
        }

        public string DesiredUsername
        {
            get => desiredUsername;
            set
            {
                desiredUsername = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(DesiredUsername));
                }
                Validate(validator, this);
                OnPropertyChanged(nameof(DesiredUsernameError));
            }
        }
        public string LobbyCodeError
        {
            get
            {
                var errors = GetErrors(nameof(LobbyCode)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string GuestEmailError
        {
            get
            {
                var errors = GetErrors(nameof(GuestEmail)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string DesiredUsernameError
        {
            get
            {
                var errors = GetErrors(nameof(DesiredUsername)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public ICommand JoinAsGuestCommand { get; }
        public ICommand GoBackCommand { get; }

        public GuestJoinViewModel()
        {
            this.validator = new GuestJoinValidator();
            Validate(validator, this);
        }

        public GuestJoinViewModel(
            IMatchmakingService matchmakingService,
            IDialogService dialogService,
            GuestJoinValidator validator,
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

            JoinAsGuestCommand = new RelayCommand(async param => await executeJoinAsGuestAsync(), param => canExecuteJoin());
            GoBackCommand = new RelayCommand(param => executeGoBack(), param => !IsBusy);

            Validate(validator, this);
        }

        private bool canExecuteJoin()
        {
            return !IsBusy && !HasErrors;
        }

        private void executeGoBack()
        {
            navigationService.goBack();
        }

        private async Task executeJoinAsGuestAsync()
        {
            MarkAllAsTouched();

            if (HasErrors) return;

            SetBusy(true);

            var joinRequest = new GuestJoinRequestDto
            {
                lobbyCode = this.LobbyCode.Trim(),
                guestEmail = this.GuestEmail.Trim(),
                desiredGuestUsername = this.DesiredUsername.Trim()
            };

            try
            {
                GuestJoinServiceResultDto serviceResult = await matchmakingService.joinLobbyAsGuestAsync(joinRequest);

                if (serviceResult.wcfResult.success && serviceResult.wcfResult.initialLobbyState != null)
                {
                    currentLobbyService.setInitialState(serviceResult.wcfResult.initialLobbyState);
                    windowNavigationService.openWindow<GameWindow>();
                    windowNavigationService.closeWindow<AuthenticationWindow>();
                }
                else
                {
                    dialogService.showError(serviceResult.wcfResult.message ?? "Unexpected error", Lang.ErrorTitle);

                    if (!serviceResult.didMatchmakingConnect)
                    {
                        matchmakingService.disconnect();
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                matchmakingService.disconnect();
            }
            catch (Exception ex)
            {
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
            string errorDetails = ex?.Message ?? Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}:\n{errorDetails}", Lang.ErrorTitle);
        }
    }
}