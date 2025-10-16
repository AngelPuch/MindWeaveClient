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

        public string email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public string verificationCode
        {
            get => verificationCodeValue;
            set
            {
                verificationCodeValue = value;
                OnPropertyChanged();
                // Llama al método que añadimos en RelayCommand para actualizar el estado del botón.
                ((RelayCommand)verifyCommand).RaiseCanExecuteChanged();
            }
        }

        public ICommand verifyCommand { get; }
        public ICommand goBackCommand { get; }
        public ICommand resendCodeCommand { get; }

        private readonly Action<Page> navigateTo;
        private readonly Action navigateBack;

        public VerificationViewModel(string email, Action<Page> navigateTo, Action navigateBack)
        {
            this.email = email;
            this.navigateTo = navigateTo;
            this.navigateBack = navigateBack;

            verifyCommand = new RelayCommand(async (param) => await executeVerifyAsync(), (param) => canExecuteVerify());
            goBackCommand = new RelayCommand((param) => executeGoBack());
            resendCodeCommand = new RelayCommand(async (param) => await executeResendCodeAsync());
        }

        private bool canExecuteVerify()
        {
            // Lógica que decide si el botón "Verificar cuenta" está habilitado.
            return !string.IsNullOrWhiteSpace(verificationCode)
                   && verificationCode.Length == 6
                   && verificationCode.All(char.IsDigit);
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
                OperationResultDto result = await client.verifyAccountAsync(email, verificationCode);

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
                // Llamamos al nuevo método del servicio que actualizaste
                OperationResultDto result = await client.resendVerificationCodeAsync(email);

                if (result.success)
                {
                    // Usamos la clave de recurso que añadiremos en el siguiente paso
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