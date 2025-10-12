using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using System;
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
        public string verificationCode { get => verificationCodeValue; set { verificationCodeValue = value; OnPropertyChanged(); } }

        public ICommand verifyCommand { get; }
        public ICommand goBackCommand { get; }

        private Action<Page> navigateTo;
        private Action navigateBack;

        public VerificationViewModel(string email, Action<Page> navigateTo, Action navigateBack)
        {
            email = email;
            this.navigateTo = navigateTo;
            this.navigateBack = navigateBack;
            verifyCommand = new RelayCommand(async (param) => await executeVerify());
            goBackCommand = new RelayCommand((param) => executeGoBack());
        }

        private async Task executeVerify()
        {
            if (string.IsNullOrWhiteSpace(verificationCode) || verificationCode.Length != 6)
            {
                MessageBox.Show("Please enter a valid 6-digit code.", "Invalid Code", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.verifyAccountAsync(email, verificationCode);

                if (result.success)
                {
                    MessageBox.Show(result.message, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.navigateTo(new LoginPage()); // Navegar a Login
                }
                else
                {
                    MessageBox.Show(result.message, "Verification Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void executeGoBack()
        {
            this.navigateBack?.Invoke();
        }
    }
}