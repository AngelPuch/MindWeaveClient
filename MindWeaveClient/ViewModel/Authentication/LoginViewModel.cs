using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MindWeaveClient.Services;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Main;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class LoginViewModel : BaseViewModel
    {
        private string email;
        private string password;
        private bool showUnverifiedControls;

        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly LoginValidator validator;
        private readonly INavigationService navigationService;
        private readonly IWindowNavigationService windowNavigationService;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(Email));
                }

                validate(validator, this);
                OnPropertyChanged(nameof(EmailError));
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(Password));
                }

                validate(validator, this);
                OnPropertyChanged(nameof(PasswordError));
            }
        }

        public string EmailError
        {
            get
            {
                var errors = GetErrors(nameof(Email)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string PasswordError
        {
            get
            {
                var errors = GetErrors(nameof(Password)) as List<string>;
                return errors?.FirstOrDefault();
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

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }
        public ICommand ForgotPasswordCommand { get; }
        public ICommand GuestLoginCommand { get; }
        public ICommand ResendVerificationCommand { get; }

        public LoginViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            LoginValidator validator,
            INavigationService navigationService,
            IWindowNavigationService windowNavigationService)
        {
            this.authenticationService = authenticationService;
            this.dialogService = dialogService;
            this.validator = validator;
            this.navigationService = navigationService;
            this.windowNavigationService = windowNavigationService;

            LoginCommand = new RelayCommand(async (param) => await executeLoginAsync(), (param) => canExecuteLogin());
            SignUpCommand = new RelayCommand((param) => executeGoToSignUp());
            ForgotPasswordCommand = new RelayCommand((param) => executeGoToForgotPassword());
            GuestLoginCommand = new RelayCommand((param) => executeGoToGuestJoin());
            ResendVerificationCommand = new RelayCommand(async (param) => await executeResendVerificationAsync());

            validate(validator, this);
        }

        private bool canExecuteLogin()
        {
            return !IsBusy && !HasErrors;
        }

        private async Task executeLoginAsync()
        {
            markAllAsTouched();

            if (HasErrors)
            {
                return;
            }

            SetBusy(true);
            try
            {
                LoginServiceResultDto serviceResult = await authenticationService.loginAsync(Email, Password);

                if (serviceResult.WcfLoginResult.operationResult.success)
                {
                    if (!serviceResult.IsSocialServiceConnected)
                    {
                        dialogService.showWarning(Lang.WarningMsgSocialConnectFailed, Lang.WarningTitle);
                    }

                    if (!serviceResult.IsMatchmakingServiceConnected)
                    {
                        dialogService.showWarning(Lang.WarningMsgMatchmakingConnectFailed, Lang.WarningTitle);
                    }

                    windowNavigationService.openWindow<MainWindow>();
                    windowNavigationService.closeWindow<AuthenticationWindow>();
                }
                else
                {
                    if (serviceResult.WcfLoginResult.resultCode == "ACCOUNT_NOT_VERIFIED")
                    {
                        ShowUnverifiedControls = true;
                    }
                    else
                    {
                        dialogService.showError(serviceResult.WcfLoginResult.operationResult.message, Lang.ErrorTitle);
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
                    SessionService.PendingVerificationEmail = this.Email;
                    navigationService.navigateTo<VerificationPage>();
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
            navigationService.navigateTo<CreateAccountPage>();
        }

        private void executeGoToForgotPassword()
        {
            navigationService.navigateTo<PasswordRecoveryPage>();
        }

        private void executeGoToGuestJoin()
        {
            navigationService.navigateTo<GuestJoinPage>();
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}