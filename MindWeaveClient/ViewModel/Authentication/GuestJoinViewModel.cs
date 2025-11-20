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
                    markAsTouched(nameof(LobbyCode));
                }
                validate(validator, this);
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
                    markAsTouched(nameof(GuestEmail));
                }
                validate(validator, this);
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
                    markAsTouched(nameof(DesiredUsername));
                }
                validate(validator, this);
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
            validate(validator, this);
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

            validate(validator, this);
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
            markAllAsTouched();
            if (HasErrors) return;
            SetBusy(true);

            var joinRequest = new GuestJoinRequestDto
            {
                LobbyCode = this.LobbyCode.Trim(),
                GuestEmail = this.GuestEmail.Trim(),
                DesiredGuestUsername = this.DesiredUsername.Trim()
            };

            try
            {
                GuestJoinServiceResultDto serviceResult = await matchmakingService.joinLobbyAsGuestAsync(joinRequest);

                if (serviceResult.WcfResult.Success && serviceResult.WcfResult.InitialLobbyState != null)
                {
                    currentLobbyService.setInitialState(serviceResult.WcfResult.InitialLobbyState);
                    windowNavigationService.openWindow<GameWindow>();
                    windowNavigationService.closeWindow<AuthenticationWindow>();
                }
                else
                {
                    dialogService.showError(serviceResult.WcfResult.Message, Lang.ErrorTitle);
                    if (!serviceResult.DidMatchmakingConnect) { matchmakingService.disconnect(); }
                }
            }
            catch (FaultException<ServiceFaultDto> ex)
            {
                dialogService.showError(ex.Detail.Message, Lang.ErrorTitle);
                matchmakingService.disconnect();
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                matchmakingService.disconnect();
            }
            catch (TimeoutException ex)
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