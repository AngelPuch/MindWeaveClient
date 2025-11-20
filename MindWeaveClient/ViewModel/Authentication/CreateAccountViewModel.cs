using MindWeaveClient.AuthenticationService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MindWeaveClient.Services;

namespace MindWeaveClient.ViewModel.Authentication
{
    public class CreateAccountViewModel : BaseViewModel
    {
        private string firstName;
        private string lastName;
        private string username;
        private string email;
        private DateTime? birthDate = DateTime.Now;
        private string password;
        private bool isFemale;
        private bool isMale;
        private bool isOther;
        private bool isPreferNotToSay;

        private readonly IAuthenticationService authenticationService;
        private readonly IDialogService dialogService;
        private readonly CreateAccountValidator validator;
        private readonly INavigationService navigationService;

        public string FirstName
        {
            get => firstName;
            set
            {
                firstName = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(FirstName));
                }
                validate(validator, this);
                OnPropertyChanged(nameof(FirstNameError));
            }
        }

        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(LastName));
                }
                validate(validator, this);
                OnPropertyChanged(nameof(LastNameError));
            }
        }

        public string Username
        {
            get => username;
            set
            {
                username = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(Username));
                }
                validate(validator, this);
                OnPropertyChanged(nameof(UsernameError));
            }
        }

        public string Email
        {
            get => email;
            set
            {
                email = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(Email));
                }
                validate(validator, this);
                OnPropertyChanged(nameof(EmailError));
            }
        }

        public DateTime? BirthDate
        {
            get => birthDate;
            set
            {
                birthDate = value;
                OnPropertyChanged();
                if (value.HasValue)
                {
                    markAsTouched(nameof(BirthDate));
                }
                validate(validator, this);
                OnPropertyChanged(nameof(BirthDateError));
            }
        }

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(Password));
                }
                validate(validator, this);
                OnPropertyChanged(nameof(PasswordError));
            }
        }

        public bool IsFemale
        {
            get => isFemale;
            set
            {
                isFemale = value;
                OnPropertyChanged();
                markAsTouched(nameof(IsFemale));
                validate(validator, this);
                OnPropertyChanged(nameof(GenderError));
            }
        }

        public bool IsMale
        {
            get => isMale;
            set
            {
                isMale = value;
                OnPropertyChanged();
                markAsTouched(nameof(IsFemale));
                validate(validator, this);
                OnPropertyChanged(nameof(GenderError));
            }
        }

        public bool IsOther
        {
            get => isOther;
            set
            {
                isOther = value;
                OnPropertyChanged();
                markAsTouched(nameof(IsFemale));
                validate(validator, this);
                OnPropertyChanged(nameof(GenderError));
            }
        }

        public bool IsPreferNotToSay
        {
            get => isPreferNotToSay;
            set
            {
                isPreferNotToSay = value;
                OnPropertyChanged();
                markAsTouched(nameof(IsFemale));
                validate(validator, this);
                OnPropertyChanged(nameof(GenderError));
            }
        }

        public string FirstNameError
        {
            get
            {
                var errors = GetErrors(nameof(FirstName)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string LastNameError
        {
            get
            {
                var errors = GetErrors(nameof(LastName)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string UsernameError
        {
            get
            {
                var errors = GetErrors(nameof(Username)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string EmailError
        {
            get
            {
                var errors = GetErrors(nameof(Email)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string BirthDateError
        {
            get
            {
                var errors = GetErrors(nameof(BirthDate)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string PasswordError
        {
            get
            {
                var errors = GetErrors(nameof(Password)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string GenderError
        {
            get
            {
                var errors = GetErrors(nameof(IsFemale)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public ICommand SignUpCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public CreateAccountViewModel()
        {
            this.validator = new CreateAccountValidator();
            validate(validator, this);
        }

        public CreateAccountViewModel(
            IAuthenticationService authenticationService,
            IDialogService dialogService,
            CreateAccountValidator validator,
            INavigationService navigationService)
        {
            this.authenticationService = authenticationService;
            this.dialogService = dialogService;
            this.validator = validator;
            this.navigationService = navigationService;

            SignUpCommand = new RelayCommand(async (param) => await executeSignUp(), (param) => canExecuteSignUp());
            GoToLoginCommand = new RelayCommand((param) => executeGoToLogin());

            validate(validator, this);
        }

        private bool canExecuteSignUp()
        {
            return !IsBusy && !HasErrors;
        }

        private async Task executeSignUp()
        {
            markAllAsTouched();

            if (HasErrors)
            {
                return;
            }

            SetBusy(true);

            var userProfile = new UserProfileDto
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                Username = this.Username,
                Email = this.Email,
                DateOfBirth = BirthDate.Value,
                GenderId = getSelectedGenderId()
            };

            try
            {
                OperationResultDto result = await authenticationService.registerAsync(userProfile, this.Password);

                if (result.Success)
                {
                    SessionService.PendingVerificationEmail = this.Email;
                    navigationService.navigateTo<VerificationPage>();
                }
                else { dialogService.showError(result.Message, Lang.ErrorTitle); }
            }
            catch (FaultException<ServiceFaultDto> ex)
            {
                dialogService.showError(ex.Detail.Message, Lang.ErrorTitle);
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorMsgSignUpFailed, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void executeGoToLogin()
        {
            navigationService.navigateTo<LoginPage>();
        }

        private int getSelectedGenderId()
        {
            if (IsFemale) return 1;
            if (IsMale) return 2;
            if (IsOther) return 3;
            return 4;
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}