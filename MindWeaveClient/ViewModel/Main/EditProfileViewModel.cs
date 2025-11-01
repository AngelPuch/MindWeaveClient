using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class EditProfileViewModel : BaseViewModel
    {
        private readonly Action navigateBack;
        private readonly Action navigateToSelectAvatar;
        private ProfileManagerClient profileClient;

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

        public string FirstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string LastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public DateTime? DateOfBirth { get => dateOfBirthValue; set { dateOfBirthValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public GenderDto SelectedGender { get => selectedGenderValue; set { selectedGenderValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public ObservableCollection<GenderDto> Genders { get => gendersValue; set { gendersValue = value; OnPropertyChanged(); } }
        public string AvatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }

        public bool IsChangePasswordSectionVisible { get => isChangePasswordSectionVisibleValue; set { isChangePasswordSectionVisibleValue = value; OnPropertyChanged(); } }
        public string CurrentPassword { get => currentPasswordValue; set { currentPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string NewPassword { get => newPasswordValue; set { newPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string ConfirmPassword { get => confirmPasswordValue; set { confirmPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public bool IsBusy { get => isBusyValue; private set { setBusy(value); } }

        public bool CanSaveChanges =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(LastName) &&
            DateOfBirth.HasValue &&
            SelectedGender != null;

        public bool CanSaveNewPassword =>
            !IsBusy &&
            !string.IsNullOrWhiteSpace(CurrentPassword) &&
            !string.IsNullOrWhiteSpace(NewPassword) &&
            !string.IsNullOrWhiteSpace(ConfirmPassword) &&
            NewPassword == ConfirmPassword;

        public ICommand SaveChangesCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ChangeAvatarCommand { get; }
        public ICommand ShowChangePasswordCommand { get; }
        public ICommand SaveNewPasswordCommand { get; }
        public ICommand CancelChangePasswordCommand { get; }


        public EditProfileViewModel(Action navigateBack, Action navigateToSelectAvatar)
        {
            this.navigateBack = navigateBack;
            this.navigateToSelectAvatar = navigateToSelectAvatar;
            this.profileClient = new ProfileManagerClient();

            CancelCommand = new RelayCommand(p => this.navigateBack?.Invoke(), p => !IsBusy);
            SaveChangesCommand = new RelayCommand(async p => await saveProfileChangesAsync(), p => CanSaveChanges);
            ChangeAvatarCommand = new RelayCommand(p => this.navigateToSelectAvatar?.Invoke(), p => !IsBusy);
            ShowChangePasswordCommand = new RelayCommand(executeShowChangePassword, p => !IsBusy);
            SaveNewPasswordCommand = new RelayCommand(async p => await executeSaveNewPasswordAsync(), p => CanSaveNewPassword);
            CancelChangePasswordCommand = new RelayCommand(executeCancelChangePassword, p => !IsBusy);

            Genders = new ObservableCollection<GenderDto>();
            loadEditableData();
        }

        private void executeShowChangePassword(object parameter)
        {
            IsChangePasswordSectionVisible = true;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        private void executeCancelChangePassword(object parameter)
        {
            IsChangePasswordSectionVisible = false;
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmPassword = string.Empty;
        }

        private async Task executeSaveNewPasswordAsync()
        {
            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show(Lang.ValidationPasswordsDoNotMatch, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
            {
                MessageBox.Show(Lang.GlobalErrorAllFieldsRequired, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            setBusy(true);
            try
            {
                var result = await profileClient.changePasswordAsync(SessionService.Username, CurrentPassword, NewPassword);

                if (result.success)
                {
                    MessageBox.Show(result.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information);
                    executeCancelChangePassword(null);
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                handleError("Error changing password", ex);
            }
            finally
            {
                setBusy(false);
            }
        }


        private async void loadEditableData()
        {
            setBusy(true);
            try
            {
                AvatarSource = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
                var profileData = await profileClient.getPlayerProfileForEditAsync(SessionService.Username);
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
                    MessageBox.Show(Lang.ErrorFailedToLoadProfile, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                raiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                handleError(Lang.ErrorFailedToLoadProfile, ex);
                navigateBack?.Invoke();
            }
            finally
            {
                setBusy(false);
            }
        }

        private async Task saveProfileChangesAsync()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || !DateOfBirth.HasValue || SelectedGender == null)
            {
                MessageBox.Show(Lang.GlobalErrorAllFieldsRequired, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
                var result = await profileClient.updateProfileAsync(SessionService.Username, updatedProfile);

                if (result.success)
                {
                    MessageBox.Show("Profile updated successfully!", Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information); // TODO: Lang key
                    navigateBack?.Invoke();
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                handleError("Error updating profile", ex);
            }
            finally
            {
                setBusy(false);
            }
        }

        public void RefreshAvatar()
        {
            AvatarSource = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        private void setBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(IsBusy));
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
            string errorDetails = ex?.Message ?? "No details provided.";
            Console.WriteLine($"!!! {message}: {errorDetails}");
            MessageBox.Show($"{message}:\n{errorDetails}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}