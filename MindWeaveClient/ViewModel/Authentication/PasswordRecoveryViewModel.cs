using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Utilities.Implementations;
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
    public class PasswordRecoveryViewModel : BaseViewModel
    {
        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly PasswordRecoveryValidator validator;
        private readonly INavigationService navigationService;

        private bool isStep1Visible = true;
        private bool isStep2Visible;
        private bool isStep3Visible;

        public bool IsStep1Visible
        {
            get => isStep1Visible;
            set
            {
                isStep1Visible = value;
                OnPropertyChanged();
            }
        }

        public bool IsStep2Visible
        {
            get => isStep2Visible;
            set
            {
                isStep2Visible = value;
                OnPropertyChanged();
            }
        }

        public bool IsStep3Visible
        {
            get => isStep3Visible;
            set
            {
                isStep3Visible = value;
                OnPropertyChanged();
            }
        }

        private string email;
        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(Email));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(EmailError));
            }
        }

        private string verificationCode;
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
                validateCurrentStep();
                OnPropertyChanged(nameof(VerificationCodeError));
            }
        }

        private string newPassword;
        public string NewPassword
        {
            get => newPassword;
            set
            {
                newPassword = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(NewPassword));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(NewPasswordError));
            }
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get => confirmPassword;
            set
            {
                confirmPassword = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(ConfirmPassword));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(ConfirmPasswordError));
            }
        }

        // Propiedades para obtener el primer error visible de cada campo
        public string EmailError
        {
            get
            {
                var errors = GetErrors(nameof(Email)) as List<string>;
                return errors?.FirstOrDefault();
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

        public string NewPasswordError
        {
            get
            {
                var errors = GetErrors(nameof(NewPassword)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string ConfirmPasswordError
        {
            get
            {
                var errors = GetErrors(nameof(ConfirmPassword)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

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
                // Marcar campo de email como tocado al intentar enviar
                MarkAsTouched(nameof(Email));
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

                        // Limpiar el estado touched del paso anterior
                        ClearTouchedState();
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
            // Marcar campo de código como tocado al intentar verificar
            MarkAsTouched(nameof(VerificationCode));
            if (HasErrors) return Task.CompletedTask;

            SetBusy(true);

            IsStep1Visible = false;
            IsStep2Visible = false;
            IsStep3Visible = true;
            NewPassword = "";
            ConfirmPassword = "";

            // Limpiar el estado touched del paso anterior
            ClearTouchedState();
            validateCurrentStep();

            SetBusy(false);
            return Task.CompletedTask;
        }

        private async Task executeSavePasswordAsync()
        {
            // Marcar campos de contraseña como tocados al intentar guardar
            MarkAsTouched(nameof(NewPassword));
            MarkAsTouched(nameof(ConfirmPassword));

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

                        // Limpiar el estado touched al retroceder
                        ClearTouchedState();
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

                // Limpiar el estado touched al retroceder
                ClearTouchedState();
                validateCurrentStep();
            }
            else if (IsStep2Visible)
            {
                IsStep1Visible = true;
                IsStep2Visible = false;
                IsStep3Visible = false;

                // Limpiar el estado touched al retroceder
                ClearTouchedState();
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