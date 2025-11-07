using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Authentication;
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class PasswordRecoveryViewModel : BaseViewModel
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly PasswordRecoveryValidator validator;
        private readonly INavigationService navigationService;

        private bool isStep1Visible = true;
        private bool isStep2Visible;
        private bool isStep3Visible;

        public bool IsStep1Visible { get => isStep1Visible; set { isStep1Visible = value; OnPropertyChanged(); } }
        public bool IsStep2Visible { get => isStep2Visible; set { isStep2Visible = value; OnPropertyChanged(); } }
        public bool IsStep3Visible { get => isStep3Visible; set { isStep3Visible = value; OnPropertyChanged(); } }

        private string email;
        public string Email { get => email; set { email = value; OnPropertyChanged(); validateCurrentStep(); } }

        private string verificationCode;
        public string VerificationCode { get => verificationCode; set { verificationCode = value; OnPropertyChanged(); validateCurrentStep(); } }

        private string newPassword;
        public string NewPassword { get => newPassword; set { newPassword = value; OnPropertyChanged(); validateCurrentStep(); } }

        private string confirmPassword;
        public string ConfirmPassword { get => confirmPassword; set { confirmPassword = value; OnPropertyChanged(); validateCurrentStep(); } }

        public ICommand SendCodeCommand { get; }
        public ICommand VerifyCodeCommand { get; }
        public ICommand ResendCodeCommand { get; }
        public ICommand SavePasswordCommand { get; }
        public ICommand GoBackCommand { get; }

        public PasswordRecoveryViewModel()
        {
            this.validator = new PasswordRecoveryValidator();
            validateCurrentStep();
        }

        public PasswordRecoveryViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            PasswordRecoveryValidator validator,
            INavigationService navigationService)
        {
            this.authenticationService = authenticationService;
            this.dialogService = dialogService;
            this.validator = validator;
            this.navigationService = navigationService;

            SendCodeCommand = new RelayCommand(async param => await executeSendCodeAsync(), param => !HasErrors && !IsBusy);
            VerifyCodeCommand = new RelayCommand(async param => await executeVerifyCodeAsync(), param => !HasErrors && !IsBusy);
            ResendCodeCommand = new RelayCommand(async param => await executeSendCodeAsync(true), param => !IsBusy);
            SavePasswordCommand = new RelayCommand(async param => await executeSavePasswordAsync(), param => !HasErrors && !IsBusy);
            GoBackCommand = new RelayCommand(param => executeGoBack());

            validateCurrentStep();
        }


        private async Task executeSendCodeAsync(bool isResend = false)
        {
            if (!isResend)
            {
                if (HasErrors) return;
            }

            SetBusy(true);
            try
            {
                var result = await authenticationService.sendPasswordRecoveryCodeAsync(Email);
                if (result.success)
                {
                    dialogService.showInfo(Lang.ValidationPasswordRecoveryCodeSent, Lang.InfoMsgResendSuccessTitle);

                    if (!isResend)
                    {
                        IsStep1Visible = false;
                        IsStep2Visible = true;
                        IsStep3Visible = false;
                        VerificationCode = string.Empty;
                        validateCurrentStep();
                    }
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

        private Task executeVerifyCodeAsync()
        {
            if (HasErrors) return Task.CompletedTask;

            SetBusy(true);

            IsStep1Visible = false;
            IsStep2Visible = false;
            IsStep3Visible = true;
            NewPassword = "";
            ConfirmPassword = "";
            validateCurrentStep();

            SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task executeSavePasswordAsync()
        {
            if (HasErrors) return;

            SetBusy(true);
            try
            {
                var result = await authenticationService.resetPasswordWithCodeAsync(Email, VerificationCode, NewPassword);

                if (result.success)
                {
                    dialogService.showInfo(Lang.InfoMsgPasswordResetSuccessBody, Lang.InfoMsgPasswordResetSuccessTitle);
                    navigationService.navigateTo<LoginPage>();
                }
                else
                {
                    dialogService.showError(result.message, Lang.ErrorTitle);

                    if (result.message.Contains(Lang.GlobalVerificationInvalidOrExpiredCode))
                    {
                        IsStep1Visible = false;
                        IsStep2Visible = true;
                        IsStep3Visible = false;
                        VerificationCode = string.Empty;
                        validateCurrentStep();
                    }
                }
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgPasswordResetFailed, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void executeGoBack()
        {
            if (IsStep3Visible)
            {
                IsStep1Visible = false;
                IsStep2Visible = true;
                IsStep3Visible = false;
                validateCurrentStep();
            }
            else if (IsStep2Visible)
            {
                IsStep1Visible = true;
                IsStep2Visible = false;
                IsStep3Visible = false;
                validateCurrentStep();
            }
            else
            {
                navigationService.goBack();
            }
        }

        private void validateCurrentStep()
        {
            if (IsStep1Visible)
            {
                Validate(validator, this, "Step1");
            }
            else if (IsStep2Visible)
            {
                Validate(validator, this, "Step2");
            }
            else if (IsStep3Visible)
            {
                Validate(validator, this, "Step3");
            }
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}