using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class EditProfileViewModel : BaseViewModel
    {
        private const string DEFAULT_AVATAR_PATH = "/Resources/Images/Avatar/default_avatar.png";
        private const string PROFILE_RULESET_NAME = "Profile";
        private const string PASSWORD_RULESET_NAME = "Password";
        private const int MAX_LENGTH_FIRST_NAME = 45;
        private const int MAX_LENGTH_LAST_NAME = 45;
        private const int MAX_LENGTH_PASSWORD = 128;

        private readonly INavigationService navigationService;
        private readonly IProfileService profileService;
        private readonly IDialogService dialogService;
        private readonly EditProfileValidator validator;
        private readonly IServiceExceptionHandler exceptionHandler;

        private string firstNameValue;
        private string lastNameValue;
        private DateTime? dateOfBirthValue;
        private string avatarSourceValue;

        private int originalGenderId;

        private bool isChangePasswordSectionVisibleValue;
        private string currentPasswordValue;
        private string newPasswordValue;
        private string confirmPasswordValue;

        public string FirstName
        {
            get => firstNameValue;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_FIRST_NAME);

                if (firstNameValue != processedValue)
                {
                    firstNameValue = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(FirstName));
                    }
                    validateCurrentStep();
                    OnPropertyChanged(nameof(FirstNameError));
                }
            }
        }

        public string LastName
        {
            get => lastNameValue;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_LAST_NAME);

                if (lastNameValue != processedValue)
                {
                    lastNameValue = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(LastName));
                    }
                    validateCurrentStep();
                    OnPropertyChanged(nameof(LastNameError));
                }
            }
        }

        public DateTime? DateOfBirth
        {
            get => dateOfBirthValue;
            set
            {
                dateOfBirthValue = value;
                OnPropertyChanged();
                if (value.HasValue)
                {
                    markAsTouched(nameof(DateOfBirth));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(DateOfBirthError));
            }
        }

        public string AvatarSource
        {
            get => avatarSourceValue;
            set
            {
                avatarSourceValue = value;
                OnPropertyChanged();
            }
        }

        public bool IsChangePasswordSectionVisible
        {
            get => isChangePasswordSectionVisibleValue;
            set
            {
                isChangePasswordSectionVisibleValue = value;
                OnPropertyChanged();
                validateCurrentStep();
            }
        }

        public string CurrentPassword
        {
            get => currentPasswordValue;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_PASSWORD);

                if (currentPasswordValue != processedValue)
                {
                    currentPasswordValue = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(CurrentPassword));
                    }
                    validateCurrentStep();
                    OnPropertyChanged(nameof(CurrentPasswordError));
                }
            }
        }

        public string NewPassword
        {
            get => newPasswordValue;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_PASSWORD);

                if (newPasswordValue != processedValue)
                {
                    newPasswordValue = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(NewPassword));
                    }
                    validateCurrentStep();
                    OnPropertyChanged(nameof(NewPasswordError));
                }
            }
        }

        public string ConfirmPassword
        {
            get => confirmPasswordValue;
            set
            {
                string processedValue = clampString(value, MAX_LENGTH_PASSWORD);

                if (confirmPasswordValue != processedValue)
                {
                    confirmPasswordValue = processedValue;
                    OnPropertyChanged();

                    if (!string.IsNullOrEmpty(processedValue))
                    {
                        markAsTouched(nameof(ConfirmPassword));
                    }
                    validateCurrentStep();
                    OnPropertyChanged(nameof(ConfirmPasswordError));
                }
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

        public string DateOfBirthError
        {
            get
            {
                var errors = GetErrors(nameof(DateOfBirth)) as List<string>;
                return errors?.FirstOrDefault();
            }
        }

        public string CurrentPasswordError
        {
            get
            {
                var errors = GetErrors(nameof(CurrentPassword)) as List<string>;
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

        public bool CanSaveChanges => !HasErrors && !IsBusy && !IsChangePasswordSectionVisible;
        public bool CanSaveNewPassword => !HasErrors && !IsBusy && IsChangePasswordSectionVisible;

        public ICommand SaveChangesCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ChangeAvatarCommand { get; }
        public ICommand ShowChangePasswordCommand { get; }
        public ICommand SaveNewPasswordCommand { get; }
        public ICommand CancelChangePasswordCommand { get; }

        public EditProfileViewModel(
            INavigationService navigationService,
            IProfileService profileService,
            IDialogService dialogService,
            EditProfileValidator validator,
            IServiceExceptionHandler exceptionHandler)
        {
            this.navigationService = navigationService;
            this.profileService = profileService;
            this.dialogService = dialogService;
            this.validator = validator;
            this.exceptionHandler = exceptionHandler;

            CancelCommand = new RelayCommand(p => this.navigationService.goBack(), p => !IsBusy);
            SaveChangesCommand = new RelayCommand(async p => await saveProfileChangesAsync(), p => CanSaveChanges);
            ChangeAvatarCommand = new RelayCommand(p => this.navigationService.navigateTo<SelectAvatarPage>(), p => !IsBusy);
            ShowChangePasswordCommand = new RelayCommand(executeShowChangePassword, p => !IsBusy);
            SaveNewPasswordCommand = new RelayCommand(async p => await executeSaveNewPasswordAsync(), p => CanSaveNewPassword);
            CancelChangePasswordCommand = new RelayCommand(executeCancelChangePassword, p => !IsBusy);

            _ = loadEditableData();
        }

        private static string clampString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        public void refreshAvatar()
        {
            AvatarSource = SessionService.AvatarPath ?? DEFAULT_AVATAR_PATH;
        }

        private void executeShowChangePassword(object parameter)
        {
            IsChangePasswordSectionVisible = true;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            clearTouchedState();
        }

        private void executeCancelChangePassword(object parameter)
        {
            IsChangePasswordSectionVisible = false;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;

            clearTouchedState();
        }

        private async Task executeSaveNewPasswordAsync()
        {
            markAsTouched(nameof(CurrentPassword));
            markAsTouched(nameof(NewPassword));
            markAsTouched(nameof(ConfirmPassword));

            if (HasErrors) return;

            setBusy(true);
            try
            {
                var result = await profileService.changePasswordAsync(SessionService.Username, CurrentPassword, NewPassword);

                if (result.Success)
                {
                    dialogService.showInfo(result.Message, Lang.InfoMsgTitleSuccess);
                    executeCancelChangePassword(null);
                }
                else
                {
                    dialogService.showError(result.Message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.ChangePasswordOperation);
            }
            finally
            {
                setBusy(false);
            }
        }

        private async Task loadEditableData()
        {
            setBusy(true);
            try
            {
                AvatarSource = SessionService.AvatarPath ?? DEFAULT_AVATAR_PATH;
                var profileData = await profileService.getPlayerProfileForEditAsync(SessionService.Username);

                if (profileData != null)
                {
                    populateProfileData(profileData);
                }
                else
                {
                    dialogService.showWarning(Lang.ErrorFailedToLoadProfile, Lang.WarningTitle);
                }

                validateCurrentStep();
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.LoadProfileOperation);
                navigationService.goBack();
            }
            finally
            {
                setBusy(false);
            }
        }

        private async Task saveProfileChangesAsync()
        {
            markAsTouched(nameof(FirstName));
            markAsTouched(nameof(LastName));
            markAsTouched(nameof(DateOfBirth));

            if (HasErrors) return;

            var updatedProfile = new UserProfileForEditDto
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                IdGender = originalGenderId,
                AvailableGenders = null
            };

            setBusy(true);
            try
            {
                var result = await profileService.updateProfileAsync(SessionService.Username, updatedProfile);

                if (result.Success)
                {
                    dialogService.showInfo(Lang.ProfileUpdateSuccess, Lang.InfoMsgTitleSuccess);
                    navigationService.goBack();
                }
                else
                {
                    dialogService.showError(result.Message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex)
            {
                exceptionHandler.handleException(ex, Lang.UpdateProfileOperation);
            }
            finally
            {
                setBusy(false);
            }
        }

        private void populateProfileData(UserProfileForEditDto profileData)
        {
            FirstName = profileData.FirstName;
            LastName = profileData.LastName;
            DateOfBirth = profileData.DateOfBirth;
            originalGenderId = profileData.IdGender;
        }

        private void setBusy(bool value)
        {
            SetBusy(value);
            raiseCanExecuteChanged();
        }

        private void validateCurrentStep()
        {
            if (IsChangePasswordSectionVisible)
            {
                validate(validator, this, PASSWORD_RULESET_NAME);
            }
            else
            {
                validate(validator, this, PROFILE_RULESET_NAME);
            }
            raiseCanExecuteChanged();
        }

        private void raiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(CanSaveChanges));
            OnPropertyChanged(nameof(CanSaveNewPassword));
        }
    }
}