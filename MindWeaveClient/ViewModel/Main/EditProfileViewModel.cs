using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.Validators;
using MindWeaveClient.View.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class EditProfileViewModel : BaseViewModel
    {
        private const string DEFAULT_AVATAR_PATH = "/Resources/Images/Avatar/default_avatar.png";
        private const string PROFILE_RULESET_NAME = "Profile";
        private const string PASSWORD_RULESET_NAME = "Password";

        private readonly INavigationService navigationService;
        private readonly IProfileService profileService;
        private readonly IDialogService dialogService;
        private readonly EditProfileValidator validator;

        private string firstNameValue;
        private string lastNameValue;
        private DateTime? dateOfBirthValue;
        private GenderDto selectedGenderValue;
        private ObservableCollection<GenderDto> gendersValue;
        private string avatarSourceValue;

        private bool isChangePasswordSectionVisibleValue;
        private string currentPasswordValue;
        private string newPasswordValue;
        private string confirmPasswordValue;
        private bool isBusyValue;

        public string FirstName
        {
            get => firstNameValue;
            set
            {
                firstNameValue = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(FirstName));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(FirstNameError));
            }
        }

        public string LastName
        {
            get => lastNameValue;
            set
            {
                lastNameValue = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(LastName));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(LastNameError));
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
                    MarkAsTouched(nameof(DateOfBirth));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(DateOfBirthError));
            }
        }

        public GenderDto SelectedGender
        {
            get => selectedGenderValue;
            set
            {
                selectedGenderValue = value;
                OnPropertyChanged();
                if (value != null)
                {
                    MarkAsTouched(nameof(SelectedGender));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(SelectedGenderError));
            }
        }

        public ObservableCollection<GenderDto> Genders
        {
            get => gendersValue;
            set
            {
                gendersValue = value;
                OnPropertyChanged();
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
                currentPasswordValue = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(CurrentPassword));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(CurrentPasswordError));
            }
        }

        public string NewPassword
        {
            get => newPasswordValue;
            set
            {
                newPasswordValue = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(NewPassword));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(NewPasswordError));
            }
        }

        public string ConfirmPassword
        {
            get => confirmPasswordValue;
            set
            {
                confirmPasswordValue = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    MarkAsTouched(nameof(ConfirmPassword));
                }
                validateCurrentStep();
                OnPropertyChanged(nameof(ConfirmPasswordError));
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

        public string SelectedGenderError
        {
            get
            {
                var errors = GetErrors(nameof(SelectedGender)) as List<string>;
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

        public bool IsBusy
        {
            get => isBusyValue;
            private set
            {
                setBusy(value);
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
            EditProfileValidator validator)
        {
            this.navigationService = navigationService;
            this.profileService = profileService;
            this.dialogService = dialogService;
            this.validator = validator;

            CancelCommand = new RelayCommand(p => this.navigationService.goBack(), p => !IsBusy);
            SaveChangesCommand = new RelayCommand(async p => await saveProfileChangesAsync(), p => CanSaveChanges);
            ChangeAvatarCommand = new RelayCommand(p => this.navigationService.navigateTo<SelectAvatarPage>(), p => !IsBusy);
            ShowChangePasswordCommand = new RelayCommand(executeShowChangePassword, p => !IsBusy);
            SaveNewPasswordCommand = new RelayCommand(async p => await executeSaveNewPasswordAsync(), p => CanSaveNewPassword);
            CancelChangePasswordCommand = new RelayCommand(executeCancelChangePassword, p => !IsBusy);

            Genders = new ObservableCollection<GenderDto>();
            loadEditableData();
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
            
            ClearTouchedState();
        }

        private void executeCancelChangePassword(object parameter)
        {
            IsChangePasswordSectionVisible = false;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
            
            ClearTouchedState();
        }

        private async Task executeSaveNewPasswordAsync()
        {
            MarkAsTouched(nameof(CurrentPassword));
            MarkAsTouched(nameof(NewPassword));
            MarkAsTouched(nameof(ConfirmPassword));
            
            if (HasErrors) return;

            setBusy(true);
            try
            {
                var result = await profileService.changePasswordAsync(SessionService.Username, CurrentPassword, NewPassword);

                if (result.success)
                {
                    dialogService.showInfo(result.message, Lang.InfoMsgTitleSuccess);
                    executeCancelChangePassword(null);
                }
                else
                {
                    dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorChangingPassword, ex);
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
                    FirstName = profileData.firstName;
                    LastName = profileData.lastName;
                    DateOfBirth = profileData.dateOfBirth;
                    Genders.Clear();
                    if (profileData.availableGenders != null)
                    {
                        foreach (var gender in profileData.availableGenders)
                        {
                            Genders.Add(gender);
                        }
                    }
                    SelectedGender = Genders.FirstOrDefault(g => g.idGender == profileData.idGender);
                }
                else
                {
                    dialogService.showWarning(Lang.ErrorFailedToLoadProfile, Lang.WarningTitle);
                }
                
                validateCurrentStep();
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorFailedToLoadProfile, ex);
                navigationService.goBack();
            }
            finally
            {
                setBusy(false);
            }
        }

        private async Task saveProfileChangesAsync()
        {
            MarkAsTouched(nameof(FirstName));
            MarkAsTouched(nameof(LastName));
            MarkAsTouched(nameof(DateOfBirth));
            MarkAsTouched(nameof(SelectedGender));
            
            if (HasErrors) return;

            var updatedProfile = new UserProfileForEditDto
            {
                firstName = this.FirstName,
                lastName = this.LastName,
                dateOfBirth = this.DateOfBirth,
                idGender = this.SelectedGender.idGender,
                availableGenders = null
            };

            setBusy(true);
            try
            {
                var result = await profileService.updateProfileAsync(SessionService.Username, updatedProfile);

                if (result.success)
                {
                    dialogService.showInfo(Lang.ProfileUpdateSuccess, Lang.InfoMsgTitleSuccess);
                    navigationService.goBack();
                }
                else
                {
                    dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex)
            {
                handleError(Lang.ProfileUpdateError, ex);
            }
            finally
            {
                setBusy(false);
            }
        }

        private void setBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(IsBusy));
            raiseCanExecuteChanged();
        }

        private void validateCurrentStep()
        {
            if (IsChangePasswordSectionVisible)
            {
                Validate(validator, this, PASSWORD_RULESET_NAME);
            }
            else
            {
                Validate(validator, this, PROFILE_RULESET_NAME);
            }
            raiseCanExecuteChanged();
        }

        private void raiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(CanSaveChanges));
            OnPropertyChanged(nameof(CanSaveNewPassword));
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void handleError(string message, Exception ex)
        {
            Trace.TraceError($"Error in {nameof(EditProfileViewModel)}: {message} | Exception: {ex}");
            dialogService.showError(message, Lang.ErrorTitle);
        }
    }
}