using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Properties.Langs;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        private bool isStep2VisibleValue = false;
        private bool isStep3VisibleValue = false;
        private bool isBusyValue = false;

        public bool isStep1Visible { get => isStep1VisibleValue; set { isStep1VisibleValue = value; OnPropertyChanged(); } }
        public bool isStep2Visible { get => isStep2VisibleValue; set { isStep2VisibleValue = value; OnPropertyChanged(); } }
        public bool isStep3Visible { get => isStep3VisibleValue; set { isStep3VisibleValue = value; OnPropertyChanged(); } }
        public bool isBusy { get => isBusyValue; private set { isBusyValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }

        private string emailValue;
        public string email
        {
            get => emailValue;
            set { emailValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); }
        }

        private string verificationCodeValue;
        public string verificationCode
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
                RaiseCanExecuteChanged();
            }
        }

        private string newPasswordValue;
        private string confirmPasswordValue;
        public string newPassword { get => newPasswordValue; set { newPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public string confirmPassword { get => confirmPasswordValue; set { confirmPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }

        public ICommand sendCodeCommand { get; }
        public ICommand verifyCodeCommand { get; }
        public ICommand resendCodeCommand { get; }
        public ICommand savePasswordCommand { get; }
        public ICommand goBackCommand { get; }

        public bool canSendCode => !isBusy && !string.IsNullOrWhiteSpace(email);
        public bool canVerifyCode => !isBusy && verificationCode?.Length == 6 && verificationCode.All(char.IsDigit); // Usa verificationCode
        public bool canResendCode => !isBusy && !string.IsNullOrWhiteSpace(email);
        public bool canSavePassword => !isBusy &&
                                       !string.IsNullOrEmpty(newPassword) &&
                                       !string.IsNullOrEmpty(confirmPassword) &&
                                       newPassword == confirmPassword;

        public PasswordRecoveryViewModel(Action navigateBackAction, Action navigateToLoginAction)
        {
            navigateBack = navigateBackAction;
            navigateToLogin = navigateToLoginAction;
            authClient = new AuthenticationManagerClient();

            sendCodeCommand = new RelayCommand(async param => await executeSendCodeAsync(), param => canSendCode);
            verifyCodeCommand = new RelayCommand(async param => await executeVerifyCodeAsync(), param => canVerifyCode);
            resendCodeCommand = new RelayCommand(async param => await executeSendCodeAsync(true), param => canResendCode);
            savePasswordCommand = new RelayCommand(async param => await executeSavePasswordAsync(), param => canSavePassword);
            goBackCommand = new RelayCommand(param => executeGoBack());
        }

        private async Task executeSendCodeAsync(bool isResend = false)
        {
            if (!canSendCode && !isResend) return;
            if (isResend && !canResendCode) return;

            SetBusy(true);
            try
            {
                OperationResultDto result = await authClient.sendPasswordRecoveryCodeAsync(email);
                if (result.success)
                {
                    MessageBox.Show(result.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    isStep1Visible = false;
                    isStep2Visible = true;
                    isStep3Visible = false;
                    verificationCode = string.Empty;
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
                SetBusy(false);
            }
        }
        private async Task executeVerifyCodeAsync()
        {
            if (!canVerifyCode) return;

            SetBusy(true);
            try
            {
                await Task.Delay(100);

                isStep1Visible = false;
                isStep2Visible = false;
                isStep3Visible = true;
                newPassword = "";
                confirmPassword = "";
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task executeSavePasswordAsync()
        {
            if (!canSavePassword)
            {
                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmPassword))
                {
                    MessageBox.Show(Lang.GlobalErrorAllFieldsRequired, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (newPassword != confirmPassword)
                {
                    MessageBox.Show(Lang.ValidationPasswordsDoNotMatch, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return;
            }


            SetBusy(true);
            try
            {
                OperationResultDto result = await authClient.resetPasswordWithCodeAsync(email, verificationCode, newPassword);

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
                        isStep1Visible = false;
                        isStep2Visible = true;
                        isStep3Visible = false;
                        verificationCode = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void executeGoBack()
        {
            if (isStep3Visible)
            {
                isStep1Visible = false;
                isStep2Visible = true;
                isStep3Visible = false;
                newPassword = "";
                confirmPassword = "";
            }
            else if (isStep2Visible)
            {
                isStep1Visible = true;
                isStep2Visible = false;
                isStep3Visible = false;
                verificationCode = string.Empty;
            }
            else
            {
                navigateBack?.Invoke();
            }
        }

        private void RaiseCanExecuteChanged()
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                CommandManager.InvalidateRequerySuggested();
                OnPropertyChanged(nameof(canSendCode));
                OnPropertyChanged(nameof(canVerifyCode));
                OnPropertyChanged(nameof(canResendCode));
                OnPropertyChanged(nameof(canSavePassword));
            });
        }
        private void SetBusy(bool value)
        {
            isBusy = value;
        }
    }
}