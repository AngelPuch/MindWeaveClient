// MindWeaveClient/ViewModel/Main/EditProfileViewModel.cs
using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Helpers;
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

        // Profile Data
        private string firstNameValue;
        private string lastNameValue;
        private DateTime? dateOfBirthValue;
        private GenderDto selectedGenderValue;
        private ObservableCollection<GenderDto> gendersValue;
        private string avatarSourceValue;

        // Password Change State & Data
        private bool isChangePasswordSectionVisibleValue = false;
        private string currentPasswordValue;
        private string newPasswordValue;
        private string confirmPasswordValue;
        private bool isBusyValue = false;

        // Public Properties
        public string firstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public string lastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public DateTime? dateOfBirth { get => dateOfBirthValue; set { dateOfBirthValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public GenderDto selectedGender { get => selectedGenderValue; set { selectedGenderValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public ObservableCollection<GenderDto> genders { get => gendersValue; set { gendersValue = value; OnPropertyChanged(); } }
        public string avatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }

        // Password Properties
        public bool isChangePasswordSectionVisible { get => isChangePasswordSectionVisibleValue; set { isChangePasswordSectionVisibleValue = value; OnPropertyChanged(); } }
        public string currentPassword { get => currentPasswordValue; set { currentPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public string newPassword { get => newPasswordValue; set { newPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public string confirmPassword { get => confirmPasswordValue; set { confirmPasswordValue = value; OnPropertyChanged(); RaiseCanExecuteChanged(); } }
        public bool isBusy { get => isBusyValue; private set { SetBusy(value); } }

        // *** CONVERTED METHOD TO PROPERTY ***
        public bool canSaveChanges =>
            !isBusy &&
            !string.IsNullOrWhiteSpace(firstName) &&
            !string.IsNullOrWhiteSpace(lastName) &&
            dateOfBirth.HasValue &&
            selectedGender != null;

        // *** CONVERTED METHOD TO PROPERTY ***
        public bool canSaveNewPassword =>
            !isBusy &&
            !string.IsNullOrWhiteSpace(currentPassword) &&
            !string.IsNullOrWhiteSpace(newPassword) &&
            !string.IsNullOrWhiteSpace(confirmPassword) &&
            newPassword == confirmPassword;

        // Commands
        public ICommand saveChangesCommand { get; }
        public ICommand cancelCommand { get; }
        public ICommand changeAvatarCommand { get; }
        public ICommand showChangePasswordCommand { get; }
        public ICommand saveNewPasswordCommand { get; }
        public ICommand cancelChangePasswordCommand { get; }


        public EditProfileViewModel(Action navigateBack, Action navigateToSelectAvatar)
        {
            this.navigateBack = navigateBack;
            this.navigateToSelectAvatar = navigateToSelectAvatar;
            this.profileClient = new ProfileManagerClient();

            cancelCommand = new RelayCommand(p => this.navigateBack?.Invoke(), p => !isBusy);
            // Use property for CanExecute
            saveChangesCommand = new RelayCommand(async p => await saveProfileChangesAsync(), p => canSaveChanges);
            changeAvatarCommand = new RelayCommand(p => this.navigateToSelectAvatar?.Invoke(), p => !isBusy);

            // Password Commands
            showChangePasswordCommand = new RelayCommand(executeShowChangePassword, p => !isBusy);
            // Use property for CanExecute
            saveNewPasswordCommand = new RelayCommand(async p => await executeSaveNewPasswordAsync(), p => canSaveNewPassword);
            cancelChangePasswordCommand = new RelayCommand(executeCancelChangePassword, p => !isBusy);

            genders = new ObservableCollection<GenderDto>();
            loadEditableData();
        }

        private void executeShowChangePassword(object parameter)
        {
            isChangePasswordSectionVisible = true;
            currentPassword = string.Empty;
            newPassword = string.Empty;
            confirmPassword = string.Empty;
        }

        private void executeCancelChangePassword(object parameter)
        {
            isChangePasswordSectionVisible = false;
            currentPassword = string.Empty;
            newPassword = string.Empty;
            confirmPassword = string.Empty;
        }

        private async Task executeSaveNewPasswordAsync()
        {
            // CanExecute property handles the check before command execution,
            // but keep internal checks for robustness or more specific feedback
            if (newPassword != confirmPassword)
            {
                MessageBox.Show(Lang.ValidationPasswordsDoNotMatch, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                MessageBox.Show(Lang.GlobalErrorAllFieldsRequired, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SetBusy(true);
            try
            {
                var result = await profileClient.changePasswordAsync(SessionService.username, currentPassword, newPassword);

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
                HandleError("Error changing password", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }


        private async void loadEditableData()
        {
            SetBusy(true);
            try
            {
                avatarSource = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
                var profileData = await profileClient.getPlayerProfileForEditAsync(SessionService.username);
                if (profileData != null)
                {
                    firstName = profileData.firstName;
                    lastName = profileData.lastName;
                    dateOfBirth = profileData.dateOfBirth;
                    genders.Clear();
                    if (profileData.availableGenders != null)
                    {
                        foreach (var gender in profileData.availableGenders)
                        {
                            genders.Add(gender);
                        }
                    }
                    selectedGender = genders.FirstOrDefault(g => g.idGender == profileData.idGender);
                }
                else
                {
                    MessageBox.Show(Lang.ErrorFailedToLoadProfile, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                // Force re-evaluation after loading
                RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                HandleError(Lang.ErrorFailedToLoadProfile, ex);
                navigateBack?.Invoke();
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task saveProfileChangesAsync()
        {
            // CanExecute property handles the check, but keep internal for feedback
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || !dateOfBirth.HasValue || selectedGender == null)
            {
                MessageBox.Show(Lang.GlobalErrorAllFieldsRequired, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var updatedProfile = new UserProfileForEditDto
            {
                firstName = this.firstName,
                lastName = this.lastName,
                dateOfBirth = this.dateOfBirth,
                idGender = this.selectedGender.idGender,
                availableGenders = null
            };

            SetBusy(true);
            try
            {
                var result = await profileClient.updateProfileAsync(SessionService.username, updatedProfile);

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
                HandleError("Error updating profile", ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        public void RefreshAvatar()
        {
            avatarSource = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        private void SetBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(isBusy));
            RaiseCanExecuteChanged(); // Notify all commands
        }

        // Helper to notify property changes for CanExecute properties
        private void RaiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(canSaveChanges));
            OnPropertyChanged(nameof(canSaveNewPassword));
            // Invalidate commands to re-evaluate CanExecute
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }


        private void HandleError(string message, Exception ex)
        {
            string errorDetails = ex?.Message ?? "No details provided.";
            Console.WriteLine($"!!! {message}: {errorDetails}");
            MessageBox.Show($"{message}:\n{errorDetails}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}