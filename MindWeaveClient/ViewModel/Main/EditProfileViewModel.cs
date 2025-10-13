// MindWeaveClient/ViewModel/Main/EditProfileViewModel.cs

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

        // --- Backing Fields (con 'Value' al final) ---
        private string firstNameValue;
        private string lastNameValue;
        private DateTime? dateOfBirthValue;
        private GenderDto selectedGenderValue;
        private ObservableCollection<GenderDto> gendersValue;
        private string avatarSourceValue;

        // --- Public Properties (camelCase) ---
        public string firstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); } }
        public string lastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); } }
        public DateTime? dateOfBirth { get => dateOfBirthValue; set { dateOfBirthValue = value; OnPropertyChanged(); } }
        public GenderDto selectedGender { get => selectedGenderValue; set { selectedGenderValue = value; OnPropertyChanged(); } }
        public ObservableCollection<GenderDto> genders { get => gendersValue; set { gendersValue = value; OnPropertyChanged(); } }
        public string avatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }

        // --- Comandos (camelCase) ---
        public ICommand saveChangesCommand { get; }
        public ICommand cancelCommand { get; }
        public ICommand changeAvatarCommand { get; }
        public ICommand changePasswordCommand { get; }

        public EditProfileViewModel(Action navigateBack, Action navigateToSelectAvatar)
        {
            this.navigateBack = navigateBack;
            this.navigateToSelectAvatar = navigateToSelectAvatar;

            cancelCommand = new RelayCommand(p => this.navigateBack?.Invoke());

            saveChangesCommand = new RelayCommand(async p => await saveChanges(), p => canSaveChanges());

            changeAvatarCommand = new RelayCommand(p => this.navigateToSelectAvatar?.Invoke());

            changePasswordCommand = new RelayCommand(p => MessageBox.Show("Change password not implemented yet."));

            genders = new ObservableCollection<GenderDto>();
            loadEditableData();
        }

        private async void loadEditableData()
        {
            try
            {
                avatarSource = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
                var client = new ProfileManagerClient();
     
                var profileData = await client.getPlayerProfileForEditAsync(SessionService.username);
                if (profileData != null)
                {
                    firstName = profileData.firstName;
                    lastName = profileData.lastName;
                    dateOfBirth = profileData.dateOfBirth;
                    genders = new ObservableCollection<GenderDto>(profileData.availableGenders);
                    selectedGender = genders.FirstOrDefault(g => g.idGender == profileData.idGender);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Lang.ErrorFailedToLoadProfile, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                navigateBack?.Invoke();
            }
        }

        private bool canSaveChanges()
        {
            return !string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && selectedGender != null;
        }

        private async Task saveChanges()
        {
            var updatedProfile = new UserProfileForEditDto
            {
                firstName = this.firstName,
                lastName = this.lastName,
                dateOfBirth = this.dateOfBirth,
                idGender = this.selectedGender.idGender,
                availableGenders = null
            };

            try
            {
                var client = new ProfileManagerClient();
                // Si aquí te da error, actualiza la referencia de servicio.
                var result = await client.updateProfileAsync(SessionService.username, updatedProfile);

                // Usamos la propiedad 'success' que corregimos en el servidor
                if (result.success)
                {
                    MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information); // TO-DO: Usar Lang
                    navigateBack?.Invoke();
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}