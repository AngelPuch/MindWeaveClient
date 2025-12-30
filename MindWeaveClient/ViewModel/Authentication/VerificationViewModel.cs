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
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class VerificationViewModel : BaseViewModel
    {
        private const int MAX_LENGTH_EMAIL = 45;
        private const int MAX_LENGTH_CODE = 6;

        private string email;
        private string verificationCode;

        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly INavigationService navigationService;
        private readonly VerificationValidator validator;
        private readonly IServiceExceptionHandler exceptionHandler;

        public string Email
        {
            get => email;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_EMAIL);
                if (email != processedValue)
                {
                    email = processedValue;
                    OnPropertyChanged();
                }
            }
        }

        public string VerificationCode
        {
            get => verificationCode;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_CODE);

                if (verificationCode != processedValue)
                {
                    verificationCode = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(VerificationCode));
                    }

                    validate(validator, this);
                    OnPropertyChanged(nameof(VerificationCodeError));
                }
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
            validate(validator, this);
        }

        public VerificationViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            INavigationService navigationService,
            VerificationValidator validator,
            IServiceExceptionHandler exceptionHandler)
        {
            this.authenticationService = authenticationService;
            this.dialogService = dialogService;
            this.navigationService = navigationService;
            this.validator = validator;
            this.exceptionHandler = exceptionHandler;

            this.Email = SessionService.PendingVerificationEmail;

            VerifyCommand = new RelayCommand(async (param) => await executeVerifyAsync(), (param) => canExecuteVerify());
            GoBackCommand = new RelayCommand((param) => executeGoBack());
            ResendCodeCommand = new RelayCommand(async (param) => await executeResendCodeAsync());

            validate(validator, this);
        }

        private static string clampString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }

        private bool canExecuteVerify()
        {
            return !IsBusy && !HasErrors;
        }

        private async Task executeVerifyAsync()
        {
            markAsTouched(nameof(VerificationCode));

            if (HasErrors) return;

            SetBusy(true);
            try
            {
                OperationResultDto result = await authenticationService.verifyAccountAsync(Email, VerificationCode);

                if (result.Success)
                {
                    dialogService.showInfo(Lang.InfoMsgVerifySuccessBody, Lang.InfoMsgVerifySuccessTitle);
                    SessionService.PendingVerificationEmail = null;
                    navigationService.navigateTo<LoginPage>();
                    executeGoBack();
                }
                else { dialogService.showError(result.Message, Lang.ErrorTitle); }
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.VerificationOperation);
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

                if (result.Success)
                {
                    dialogService.showInfo(Lang.InfoMsgResendSuccessBody, Lang.InfoMsgResendSuccessTitle);

                    VerificationCode = string.Empty;
                    clearTouchedState();
                }
                else { dialogService.showError(result.Message, Lang.ErrorTitle); }
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.ResendCodeOperation);
            }
            finally
            {
                SetBusy(false);
            }
        }
    }
}