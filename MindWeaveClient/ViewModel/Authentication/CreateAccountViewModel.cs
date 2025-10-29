using MindWeaveClient.AuthenticationService;
using MindWeaveClient.View.Authentication;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class CreateAccountViewModel : BaseViewModel
    {
        private string firstNameValue;
        private string lastNameValue;
        private string usernameValue;
        private string emailValue;
        private DateTime? birthDateValue = DateTime.Now;
        private string passwordValue;

        public string firstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); } }
        public string lastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); } }
        public string username { get => usernameValue; set { usernameValue = value; OnPropertyChanged(); } }
        public string email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public DateTime? birthDate { get => birthDateValue; set { birthDateValue = value; OnPropertyChanged(); } }
        public string password { get => passwordValue; set { passwordValue = value; OnPropertyChanged(); } }


        private bool isFemaleValue;
        private bool isMaleValue;
        private bool isOtherValue;
        private bool isPreferNotToSayValue;

        public bool isFemale { get => isFemaleValue; set { isFemaleValue = value; OnPropertyChanged(); } }
        public bool isMale { get => isMaleValue; set { isMaleValue = value; OnPropertyChanged(); } }
        public bool isOther { get => isOtherValue; set { isOtherValue = value; OnPropertyChanged(); } }
        public bool isPreferNotToSay { get => isPreferNotToSayValue; set { isPreferNotToSayValue = value; OnPropertyChanged(); } }

        public ICommand signUpCommand { get; }
        public ICommand goToLoginCommand { get; }

        private readonly Action<Page> navigateTo;

        public CreateAccountViewModel(Action<Page> navigateAction)
        {
            navigateTo = navigateAction;
            signUpCommand = new RelayCommand(async (param) => await executeSignUp());
            goToLoginCommand = new RelayCommand((param) => executeGoToLogin());
        }

        private async Task executeSignUp()
        {
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || birthDate == null)
            {
                MessageBox.Show("Please fill all required fields.", "Incomplete Form", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userProfile = new UserProfileDto
            {
                firstName = this.firstName,
                lastName = this.lastName,
                username = this.username,
                email = this.email,
                dateOfBirth = this.birthDate.Value,
                genderId = getSelectedGenderId()
            };

            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.registerAsync(userProfile, this.password);

                if (result.success)
                {
                    MessageBox.Show(result.message, "Registration Pending", MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateTo(new VerificationPage(email));
                }
                else
                {
                    MessageBox.Show(result.message, "Registration Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void executeGoToLogin()
        {
            navigateTo(new LoginPage());
        }

        private int getSelectedGenderId()
        {
            if (isFemale) return 1;
            if (isMale) return 2;
            if (isOther) return 3;
            return 4;
        }
    }
}