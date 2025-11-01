using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class VerificationViewModel : BaseViewModel
    {
        private string emailValue;
        private string verificationCodeValue;

        public string Email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public string VerificationCode
        {
            get => verificationCodeValue;
            set
            {
                verificationCodeValue = value;
                OnPropertyChanged();
                ((RelayCommand)VerifyCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand VerifyCommand { get; }
        public ICommand GoBackCommand { get; }
        public ICommand ResendCodeCommand { get; }

        private readonly Action<Page> navigateTo;
        private readonly Action navigateBack;

        public VerificationViewModel(string email, Action<Page> navigateTo, Action navigateBack)
        {
            this.Email = email;
            this.navigateTo = navigateTo;
            this.navigateBack = navigateBack;

            VerifyCommand = new RelayCommand(async (param) => await executeVerifyAsync(), (param) => canExecuteVerify());
            GoBackCommand = new RelayCommand((param) => executeGoBack());
            ResendCodeCommand = new RelayCommand(async (param) => await executeResendCodeAsync());
        }

        private bool canExecuteVerify()
        {
            return !string.IsNullOrWhiteSpace(VerificationCode)
                   && VerificationCode.Length == 6
                   && VerificationCode.All(char.IsDigit);
        }

        private async Task executeVerifyAsync()
        {
            if (!canExecuteVerify())
            {
                MessageBox.Show(Properties.Langs.Lang.VerificationCodeInvalidFormat, "Código Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.verifyAccountAsync(Email, VerificationCode);

                if (result.success)
                {
                    MessageBox.Show(result.message, "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.navigateTo(new LoginPage());
                }
                else
                {
                    MessageBox.Show(result.message, "Verificación Fallida", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void executeGoBack()
        {
            this.navigateBack?.Invoke();
        }

        private async Task executeResendCodeAsync()
        {
            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.resendVerificationCodeAsync(Email);

                if (result.success)
                {
                    MessageBox.Show("Código enviado con éxito", "Envio exitoso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(result.message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ocurrió un error de conexión: {ex.Message}", "Error de Conexión", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}