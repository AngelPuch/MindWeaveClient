// MindWeaveClient/ViewModel/Authentication/PasswordRecoveryViewModel.cs
using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Properties.Langs;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Text.RegularExpressions; // Para validación regex

namespace MindWeaveClient.ViewModel.Authentication
{
    public class PasswordRecoveryViewModel : BaseViewModel
    {
        // ... (navigateBack, navigateToLogin, authClient, State Management sin cambios) ...
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

        // --- Step 1 Properties (Sin cambios) ---
        private string emailValue;
        public string email
        {
            get => emailValue;
            set { emailValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); }
        }

        // --- Step 2 Properties (MODIFICADO) ---
        // Quitamos codeDigit1 a codeDigit6 y UpdateFullCode
        private string verificationCodeValue;
        public string verificationCode
        {
            get => verificationCodeValue;
            set
            {
                // Aseguramos que solo sean dígitos y limpiamos si no
                if (value != null && !Regex.IsMatch(value, "^[0-9]*$"))
                {
                    // Encuentra el último caracter válido o devuelve vacío
                    value = Regex.Match(verificationCodeValue ?? "", "^[0-9]*").Value;
                }
                verificationCodeValue = value;
                OnPropertyChanged();
                RaiseCanExecuteChanged(); // Actualiza CanExecute
            }
        }

        // --- Step 3 Properties (Sin cambios) ---
        private string newPasswordValue;
        private string confirmPasswordValue;
        public string newPassword { get => newPasswordValue; set { newPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public string confirmPassword { get => confirmPasswordValue; set { confirmPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }

        // --- Commands (Sin cambios en declaración) ---
        public ICommand sendCodeCommand { get; }
        public ICommand verifyCodeCommand { get; }
        public ICommand resendCodeCommand { get; }
        public ICommand savePasswordCommand { get; }
        public ICommand goBackCommand { get; }

        // --- CanExecute Properties (MODIFICADO para usar verificationCode) ---
        public bool canSendCode => !isBusy && !string.IsNullOrWhiteSpace(email);
        public bool canVerifyCode => !isBusy && verificationCode?.Length == 6 && verificationCode.All(char.IsDigit); // Usa verificationCode
        public bool canResendCode => !isBusy && !string.IsNullOrWhiteSpace(email);
        public bool canSavePassword => !isBusy &&
                                       !string.IsNullOrEmpty(newPassword) &&
                                       !string.IsNullOrEmpty(confirmPassword) &&
                                       newPassword == confirmPassword;

        // --- Constructor (Sin cambios) ---
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

        // --- executeSendCodeAsync (MODIFICADO para limpiar verificationCode) ---
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
                    verificationCode = string.Empty; // Limpia el código al entrar/reenviar
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


        // --- executeVerifyCodeAsync (Sin cambios lógicos internos, solo CanExecute cambió) ---
        private async Task executeVerifyCodeAsync()
        {
            if (!canVerifyCode) return;

            SetBusy(true);
            try
            {
                // Simulamos que la verificación fue "exitosa" para pasar al siguiente paso
                // La validación real se hará en executeSavePasswordAsync
                await Task.Delay(100); // Pequeña pausa opcional

                isStep1Visible = false;
                isStep2Visible = false;
                isStep3Visible = true;
                // Limpiar campos de contraseña por si vuelve atrás
                newPassword = "";
                confirmPassword = "";
            }
            finally
            {
                SetBusy(false);
            }
        }


        // --- executeSavePasswordAsync (MODIFICADO para usar verificationCode) ---
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
                // Usa la propiedad 'verificationCode' directamente
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
                        isStep2Visible = true; // Volver a pedir código
                        isStep3Visible = false;
                        verificationCode = string.Empty; // Limpiar el código viejo
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

        // --- executeGoBack (MODIFICADO para limpiar verificationCode) ---
        private void executeGoBack()
        {
            if (isStep3Visible)
            {
                isStep1Visible = false;
                isStep2Visible = true;
                isStep3Visible = false;
                newPassword = "";
                confirmPassword = "";
                // Mantenemos verificationCode por si el usuario solo quiere corregirlo
            }
            else if (isStep2Visible)
            {
                isStep1Visible = true;
                isStep2Visible = false;
                isStep3Visible = false;
                verificationCode = string.Empty; // Limpia el código si vuelve al paso 1
            }
            else
            {
                navigateBack?.Invoke();
            }
        }

        // --- ELIMINADO ---
        // private void ClearCodeDigits() { ... }

        // --- RaiseCanExecuteChanged (Sin cambios) ---
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
        // --- SetBusy (Sin cambios) ---
        private void SetBusy(bool value)
        {
            isBusy = value;
        }
    }
}