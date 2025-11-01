using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Properties.Langs;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class PasswordRecoveryViewModel : BaseViewModel
    {
        private readonly Action navigateBack;
        private readonly Action navigateToLogin;
        private readonly AuthenticationManagerClient authClient;

        private bool isStep1VisibleValue = true;
        private bool isStep2VisibleValue;
        private bool isStep3VisibleValue;
        private bool isBusyValue;

        public bool IsStep1Visible { get => isStep1VisibleValue; set { isStep1VisibleValue = value; OnPropertyChanged(); } }
        public bool IsStep2Visible { get => isStep2VisibleValue; set { isStep2VisibleValue = value; OnPropertyChanged(); } }
        public bool IsStep3Visible { get => isStep3VisibleValue; set { isStep3VisibleValue = value; OnPropertyChanged(); } }
        public bool IsBusy { get => isBusyValue; private set { isBusyValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }

        private string emailValue;
        public string Email
        {
            get => emailValue;
            set { emailValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); }
        }

        private string verificationCodeValue;
        public string VerificationCode
        {
            get => verificationCodeValue;
            set
            {
                if (value != null && !Regex.IsMatch(value, "^[0-9]*$"))
                {
                    value = Regex.Match(verificationCodeValue ?? "", "^[0-9]*").Value;
                }
                verificationCodeValue = value;
                OnPropertyChanged();
                raiseCanExecuteChanged();
            }
        }

        private string newPasswordValue;
        private string confirmPasswordValue;
        public string NewPassword { get => newPasswordValue; set { newPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string ConfirmPassword { get => confirmPasswordValue; set { confirmPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }

        public ICommand SendCodeCommand { get; }
        public ICommand VerifyCodeCommand { get; }
        public ICommand ResendCodeCommand { get; }
        public ICommand SavePasswordCommand { get; }
        public ICommand GoBackCommand { get; }

        public bool CanSendCode => !IsBusy && !string.IsNullOrWhiteSpace(Email);
        public bool CanVerifyCode => !IsBusy && VerificationCode?.Length == 6 && VerificationCode.All(char.IsDigit); // Usa verificationCode
        public bool CanResendCode => !IsBusy && !string.IsNullOrWhiteSpace(Email);
        public bool CanSavePassword => !IsBusy &&
                                       !string.IsNullOrEmpty(NewPassword) &&
                                       !string.IsNullOrEmpty(ConfirmPassword) &&
                                       NewPassword == ConfirmPassword;

        public PasswordRecoveryViewModel(Action navigateBackAction, Action navigateToLoginAction)
        {
            navigateBack = navigateBackAction;
            navigateToLogin = navigateToLoginAction;
            authClient = new AuthenticationManagerClient();

            SendCodeCommand = new RelayCommand(async param => await executeSendCodeAsync(), param => CanSendCode);
            VerifyCodeCommand = new RelayCommand(async param => await executeVerifyCodeAsync(), param => CanVerifyCode);
            ResendCodeCommand = new RelayCommand(async param => await executeSendCodeAsync(true), param => CanResendCode);
            SavePasswordCommand = new RelayCommand(async param => await executeSavePasswordAsync(), param => CanSavePassword);
            GoBackCommand = new RelayCommand(param => executeGoBack());
        }

        private async Task executeSendCodeAsync(bool isResend = false)
        {
            if (!CanSendCode && !isResend) return;
            if (isResend && !CanResendCode) return;

            setBusy(true);
            try
            {
                OperationResultDto result = await authClient.sendPasswordRecoveryCodeAsync(Email);
                if (result.success)
                {
                    MessageBox.Show(result.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    IsStep1Visible = false;
                    IsStep2Visible = true;
                    IsStep3Visible = false;
                    VerificationCode = string.Empty;
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                setBusy(false);
            }
        }
        private async Task executeVerifyCodeAsync()
        {
            if (!CanVerifyCode) return;

            setBusy(true);
            try
            {
                await Task.Delay(100);

                IsStep1Visible = false;
                IsStep2Visible = false;
                IsStep3Visible = true;
                NewPassword = "";
                ConfirmPassword = "";
            }
            finally
            {
                setBusy(false);
            }
        }

        private async Task executeSavePasswordAsync()
        {
            if (!CanSavePassword)
            {
                if (string.IsNullOrEmpty(NewPassword) || string.IsNullOrEmpty(ConfirmPassword))
                {
                    MessageBox.Show(Lang.GlobalErrorAllFieldsRequired, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (NewPassword != ConfirmPassword)
                {
                    MessageBox.Show(Lang.ValidationPasswordsDoNotMatch, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return;
            }


            setBusy(true);
            try
            {
                OperationResultDto result = await authClient.resetPasswordWithCodeAsync(Email, VerificationCode, NewPassword);

                if (result.success)
                {
                    MessageBox.Show(result.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateToLogin?.Invoke();
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                    if (result.message == Lang.GlobalVerificationInvalidOrExpiredCode)
                    {
                        IsStep1Visible = false;
                        IsStep2Visible = true;
                        IsStep3Visible = false;
                        VerificationCode = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                setBusy(false);
            }
        }

        private void executeGoBack()
        {
            if (IsStep3Visible)
            {
                IsStep1Visible = false;
                IsStep2Visible = true;
                IsStep3Visible = false;
                NewPassword = "";
                ConfirmPassword = "";
            }
            else if (IsStep2Visible)
            {
                IsStep1Visible = true;
                IsStep2Visible = false;
                IsStep3Visible = false;
                VerificationCode = string.Empty;
            }
            else
            {
                navigateBack?.Invoke();
            }
        }

        private void raiseCanExecuteChanged()
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
                OnPropertyChanged(nameof(CanSendCode));
                OnPropertyChanged(nameof(CanVerifyCode));
                OnPropertyChanged(nameof(CanResendCode));
                OnPropertyChanged(nameof(CanSavePassword));
            });
        }
        private void setBusy(bool value)
        {
            IsBusy = value;
        }
    }
}