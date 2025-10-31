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

        public string firstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string lastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public DateTime? dateOfBirth { get => dateOfBirthValue; set { dateOfBirthValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public GenderDto selectedGender { get => selectedGenderValue; set { selectedGenderValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public ObservableCollection<GenderDto> genders { get => gendersValue; set { gendersValue = value; OnPropertyChanged(); } }
        public string avatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }

        public bool isChangePasswordSectionVisible { get => isChangePasswordSectionVisibleValue; set { isChangePasswordSectionVisibleValue = value; OnPropertyChanged(); } }
        public string currentPassword { get => currentPasswordValue; set { currentPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string newPassword { get => newPasswordValue; set { newPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public string confirmPassword { get => confirmPasswordValue; set { confirmPasswordValue = value; OnPropertyChanged(); raiseCanExecuteChanged(); } }
        public bool isBusy { get => isBusyValue; private set { setBusy(value); } }

        public bool canSaveChanges =>
            !isBusy &&
            !string.IsNullOrWhiteSpace(firstName) &&
            !string.IsNullOrWhiteSpace(lastName) &&
            dateOfBirth.HasValue &&
            selectedGender != null;

        public bool canSaveNewPassword =>
            !isBusy &&
            !string.IsNullOrWhiteSpace(currentPassword) &&
            !string.IsNullOrWhiteSpace(newPassword) &&
            !string.IsNullOrWhiteSpace(confirmPassword) &&
            newPassword == confirmPassword;

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
            saveChangesCommand = new RelayCommand(async p => await saveProfileChangesAsync(), p => canSaveChanges);
            changeAvatarCommand = new RelayCommand(p => this.navigateToSelectAvatar?.Invoke(), p => !isBusy);
            showChangePasswordCommand = new RelayCommand(executeShowChangePassword, p => !isBusy);
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

            setBusy(true);
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

            setBusy(true);
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
                handleError("Error updating profile", ex);
            }
            finally
            {
                setBusy(false);
            }
        }

        public void refreshAvatar()
        {
            avatarSource = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        private void setBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(isBusy));
            raiseCanExecuteChanged(); 
        }

        private void raiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(canSaveChanges));
            OnPropertyChanged(nameof(canSaveNewPassword));
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