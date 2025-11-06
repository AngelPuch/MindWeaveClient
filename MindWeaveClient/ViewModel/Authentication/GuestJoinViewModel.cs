using MindWeaveClient.MatchmakingService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services; 
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
using MindWeaveClient.Validators;
using System;
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
        private readonly Action navigateBack;

        private readonly IMatchmakingService matchmakingService;
        private readonly IDialogService dialogService;
        private readonly GuestJoinValidator validator;

        public event EventHandler<GuestJoinResultDto> JoinSuccess;

        public string LobbyCode
        {
            get => lobbyCode;
            set { lobbyCode = value; OnPropertyChanged(); Validate(validator, this); }
        }

        public string GuestEmail
        {
            get => guestEmail;
            set { guestEmail = value; OnPropertyChanged(); Validate(validator, this); }
        }

        public string DesiredUsername
        {
            get => desiredUsername;
            set { desiredUsername = value; OnPropertyChanged(); Validate(validator, this); }
        }

        public ICommand JoinAsGuestCommand { get; }
        public ICommand GoBackCommand { get; }

        public GuestJoinViewModel()
        {
            this.validator = new GuestJoinValidator();
            Validate(validator, this);
        }

        public GuestJoinViewModel(Action navigateBackAction)
        {
            this.navigateBack = navigateBackAction;

            this.matchmakingService = new Services.Implementations.MatchmakingService();
            this.dialogService = new DialogService();
            this.validator = new GuestJoinValidator();

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
            navigateBack?.Invoke();
        }

        private async Task executeJoinAsGuestAsync()
        {
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
                    JoinSuccess?.Invoke(this, serviceResult.wcfResult);
                }
                else
                {
                    dialogService.showError(serviceResult.wcfResult.message ?? "Unexpected error", Lang.ErrorTitle);

                    if (!serviceResult.didMatchmakingConnect)
                    {
                        MatchmakingServiceClientManager.instance.Disconnect();
                    }
                }
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

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex?.Message ?? Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}:\n{errorDetails}", Lang.ErrorTitle);
        }
    }
}