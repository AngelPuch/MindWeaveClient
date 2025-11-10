using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class VerificationViewModel : BaseViewModel
    {
        private string email;
        private string verificationCode;

        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly VerificationValidator validator;

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
            }
        }

        public string VerificationCode
        {
            get => verificationCode;
            set
            {
                verificationCode = value;
                OnPropertyChanged();

                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(VerificationCode));
                }

                Validate(validator, this);
                OnPropertyChanged(nameof(VerificationCodeError));
            }
        }

        public string VerificationCodeError
        {
            get
            {
                var errors = GetErrors(nameof(VerificationCode)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public ICommand VerifyCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ResendCodeCommand { get; }

        public VerificationViewModel()
        {
            this.validator = new VerificationValidator();
            Validate(validator, this);
        }

        public VerificationViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            INavigationService navigationService,
            VerificationValidator validator)
        {
            this.authenticationService = authenticationService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.validator = validator;

            this.Email = SessionService.PendingVerificationEmail;

            VerifyCommand = new RelayCommand(async (param) => await executeVerifyAsync(), (param) => canExecuteVerify());
            GoBackCommand = new RelayCommand((param) => executeGoBack());
            ResendCodeCommand = new RelayCommand(async (param) => await executeResendCodeAsync());

            Validate(validator, this);
        }

        private bool canExecuteVerify()
        {
            return !IsBusy && !HasErrors;
        }

        private async Task executeVerifyAsync()
        {
            MarkAsTouched(nameof(VerificationCode));

            if (HasErrors) return;

            SetBusy(true);
            try
            {
                OperationResultDto result = await authenticationService.verifyAccountAsync(Email, VerificationCode);

                if (result.success)
                {
                    dialogService.showInfo(Lang.InfoMsgVerifySuccessBody, Lang.InfoMsgVerifySuccessTitle);
                    SessionService.PendingVerificationEmail = null;
                    navigationService.navigateTo<LoginPage>();
                    executeGoBack();
                }
                else
                {
                    dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgVerifyFailed, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void executeGoBack()
        {
            SessionService.PendingVerificationEmail = null;
            navigationService.navigateTo<LoginPage>();
        }

        private async Task executeResendCodeAsync()
        {
            SetBusy(true);
            try
            {
                OperationResultDto result = await authenticationService.resendVerificationCodeAsync(Email);

                if (result.success)
                {
                    dialogService.showInfo(Lang.InfoMsgResendSuccessBody, Lang.InfoMsgResendSuccessTitle);

                    VerificationCode = string.Empty;
                    ClearTouchedState();
                }
                else
                {
                    dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
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

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}