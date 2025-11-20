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
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
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

        public string FirstName
        {
            get => firstNameValue;
            set
            {
                firstNameValue = value;
                OnPropertyChanged();
                if (!string.IsNullOrEmpty(value))
                {
                    markAsTouched(nameof(FirstName));
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
                    markAsTouched(nameof(LastName));
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
                    markAsTouched(nameof(DateOfBirth));
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
                    markAsTouched(nameof(SelectedGender));
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
                    markAsTouched(nameof(CurrentPassword));
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
                    markAsTouched(nameof(NewPassword));
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
                    markAsTouched(nameof(ConfirmPassword));
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
            _ = loadEditableData();
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
            catch (FaultException<ServiceFaultDto> faultEx)
            {
                dialogService.showError(faultEx.Detail.Message, Lang.ErrorTitle);
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
                    FirstName = profileData.FirstName;
                    LastName = profileData.LastName;
                    DateOfBirth = profileData.DateOfBirth;
                    Genders.Clear();
                    if (profileData.AvailableGenders != null)
                    {
                        foreach (var gender in profileData.AvailableGenders)
                        {
                            Genders.Add(gender);
                        }
                    }
                    SelectedGender = Genders.FirstOrDefault(g => g.IdGender == profileData.IdGender);
                }
                else
                {
                    dialogService.showWarning(Lang.ErrorFailedToLoadProfile, Lang.WarningTitle);
                }

                validateCurrentStep();
            }
            catch (FaultException<ServiceFaultDto> faultEx)
            {
                dialogService.showError(faultEx.Detail.Message, Lang.ErrorTitle);
                navigationService.goBack();
            }
            catch (EndpointNotFoundException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                navigationService.goBack();
            }
            catch (TimeoutException ex)
            {
                handleError(Lang.ErrorMsgServerOffline, ex);
                navigationService.goBack();
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
            markAsTouched(nameof(FirstName));
            markAsTouched(nameof(LastName));
            markAsTouched(nameof(DateOfBirth));
            markAsTouched(nameof(SelectedGender));

            if (HasErrors) return;

            var updatedProfile = new UserProfileForEditDto
            {
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                IdGender = this.SelectedGender.IdGender,
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
            catch (FaultException<ServiceFaultDto> faultEx)
            {
                dialogService.showError(faultEx.Detail.Message, Lang.ErrorTitle);
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
                handleError(Lang.ProfileUpdateError, ex);
            }
            finally
            {
                setBusy(false);
            }
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

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }
    }
}