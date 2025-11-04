using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
using MindWeaveClient.View.Authentication;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MindWeaveClient.Services;
using MindWeaveClient.Validators;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private string email;
        private string password;
        private bool showUnverifiedControls = false;

        private readonly Action<Page> navigateAction;

        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly LoginValidator validator;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
                Validate(validator, this);
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
                Validate(validator, this);
            }
        }

        public bool ShowUnverifiedControls
        {
            get => showUnverifiedControls;
            set
            {
                showUnverifiedControls = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler LoginSuccess;

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand GuestLoginCommand { get; }
        public ICommand ResendVerificationCommand { get; }

        public LoginViewModel()
        {

        }

        public LoginViewModel(Action<Page> navigateAction)
        {
            this.navigateAction = navigateAction;

            this.authenticationService = new Services.Implementations.AuthenticationService();
            this.dialogService = new DialogService();
            this.validator = new LoginValidator();

            LoginCommand = new RelayCommand(async (param) => await executeLoginAsync(), (param) => canExecuteLogin());
            SignUpCommand = new RelayCommand((param) => executeGoToSignUp());
            ForgotPasswordCommand = new RelayCommand((param) => executeGoToForgotPassword());
            GuestLoginCommand = new RelayCommand((param) => executeGoToGuestJoin());
            ResendVerificationCommand = new RelayCommand(async (param) => await executeResendVerificationAsync());

            Validate(validator, this);
        }


        private bool canExecuteLogin()
        {
            return !IsBusy && !HasErrors;
        }

        private async Task executeLoginAsync()
        {
            if (HasErrors)
            {
                return;
            }

            SetBusy(true);
            try
            {
                LoginServiceResultDto serviceResult = await authenticationService.loginAsync(Email, Password);

                if (serviceResult.wcfLoginResult.operationResult.success)
                {
                    if (!serviceResult.isSocialServiceConnected)
                    {
                        dialogService.showWarning(Lang.WarningMsgSocialConnectFailed, Lang.WarningTitle);
                    }
                    if (!serviceResult.isMatchmakingServiceConnected)
                    {
                        dialogService.showWarning(Lang.WarningMsgMatchmakingConnectFailed, Lang.WarningTitle);
                    }

                    LoginSuccess?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    if (serviceResult.wcfLoginResult.resultCode == "ACCOUNT_NOT_VERIFIED")
                    {
                        ShowUnverifiedControls = true;
                    }
                    else
                    {
                        dialogService.showError(serviceResult.wcfLoginResult.operationResult.message, Lang.ErrorTitle);
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgLoginFailed, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeResendVerificationAsync()
        {
            SetBusy(true);
            try
            {
                var result = await authenticationService.resendVerificationCodeAsync(Email);

                if (result.success)
                {
                    dialogService.showInfo(Lang.InfoMsgBodyCodeSent, Lang.InfoMsgTitleSuccess);
                    navigateAction(new VerificationPage(this.Email));
                }
                else
                {
                    handleError(result.message, null);
                }
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgResendCodeFailed, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void executeGoToSignUp()
        {
            navigateAction(new CreateAccountPage());
        }

        private void executeGoToForgotPassword()
        {
            navigateAction(new PasswordRecoveryPage());
        }

        private void executeGoToGuestJoin()
        {
            navigateAction(new GuestJoinPage());
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}