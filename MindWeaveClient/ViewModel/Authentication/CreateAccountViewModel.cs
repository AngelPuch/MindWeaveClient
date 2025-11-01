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

        public string FirstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); } }
        public string LastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); } }
        public string Username { get => usernameValue; set { usernameValue = value; OnPropertyChanged(); } }
        public string Email { get => emailValue; set { emailValue = value; OnPropertyChanged(); } }
        public DateTime? BirthDate { get => birthDateValue; set { birthDateValue = value; OnPropertyChanged(); } }
        public string Password { get => passwordValue; set { passwordValue = value; OnPropertyChanged(); } }


        private bool isFemaleValue;
        private bool isMaleValue;
        private bool isOtherValue;
        private bool isPreferNotToSayValue;

        public bool IsFemale { get => isFemaleValue; set { isFemaleValue = value; OnPropertyChanged(); } }
        public bool IsMale { get => isMaleValue; set { isMaleValue = value; OnPropertyChanged(); } }
        public bool IsOther { get => isOtherValue; set { isOtherValue = value; OnPropertyChanged(); } }
        public bool IsPreferNotToSay { get => isPreferNotToSayValue; set { isPreferNotToSayValue = value; OnPropertyChanged(); } }

        public ICommand SignUpCommand { get; }
        public ICommand GoToLoginCommand { get; }

        private readonly Action<Page> navigateTo;

        public CreateAccountViewModel(Action<Page> navigateAction)
        {
            navigateTo = navigateAction;
            SignUpCommand = new RelayCommand(async (param) => await executeSignUp());
            GoToLoginCommand = new RelayCommand((param) => executeGoToLogin());
        }

        private async Task executeSignUp()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) || BirthDate == null)
            {
                MessageBox.Show("Please fill all required fields.", "Incomplete Form", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var userProfile = new UserProfileDto
            {
                firstName = this.FirstName,
                lastName = this.LastName,
                username = this.Username,
                email = this.Email,
                dateOfBirth = this.BirthDate.Value,
                genderId = getSelectedGenderId()
            };

            try
            {
                var client = new AuthenticationManagerClient();
                OperationResultDto result = await client.registerAsync(userProfile, this.Password);

                if (result.success)
                {
                    MessageBox.Show(result.message, "Registration Pending", MessageBoxButton.OK, MessageBoxImage.Information);
                    navigateTo(new VerificationPage(Email));
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
            if (IsFemale) return 1;
            if (IsMale) return 2;
            if (IsOther) return 3;
            return 4;
        }
    }
}