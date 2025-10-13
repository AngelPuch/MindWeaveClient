// MindWeaveClient/ViewModel/Main/ProfileViewModel.cs

using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly Action navigateBack;
        private readonly Action navigateToEdit;

        // --- Backing Fields ---
        private string welcomeMessageValue;
        private string usernameValue;
        private string avatarSourceValue;
        private string firstNameValue;
        private string lastNameValue;
        private string dateOfBirthValue;
        private string genderValue;
        private int puzzlesCompletedValue;
        private int puzzlesWonValue;
        private string totalPlaytimeValue;
        private int highestScoreValue;
        private ObservableCollection<AchievementDto> achievementsValue;

        // --- Public Properties (camelCase) for Data Binding ---
        public string welcomeMessage { get => welcomeMessageValue; set { welcomeMessageValue = value; OnPropertyChanged(); } }
        public string username { get => usernameValue; set { usernameValue = value; OnPropertyChanged(); } }
        public string avatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }
        public string firstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); } }
        public string lastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); } }
        public string dateOfBirth { get => dateOfBirthValue; set { dateOfBirthValue = value; OnPropertyChanged(); } }
        public string gender { get => genderValue; set { genderValue = value; OnPropertyChanged(); } }
        public int puzzlesCompleted { get => puzzlesCompletedValue; set { puzzlesCompletedValue = value; OnPropertyChanged(); } }
        public int puzzlesWon { get => puzzlesWonValue; set { puzzlesWonValue = value; OnPropertyChanged(); } }
        public string totalPlaytime { get => totalPlaytimeValue; set { totalPlaytimeValue = value; OnPropertyChanged(); } }
        public int highestScore { get => highestScoreValue; set { highestScoreValue = value; OnPropertyChanged(); } }
        public ObservableCollection<AchievementDto> achievements { get => achievementsValue; set { achievementsValue = value; OnPropertyChanged(); } }

        public ICommand backCommand { get; }
        public ICommand editProfileCommand { get; }

      
            public ProfileViewModel(Action navigateBack, Action navigateToEdit)
        {
            this.navigateBack = navigateBack;
            this.navigateToEdit = navigateToEdit;

            backCommand = new RelayCommand(p => navigateBack?.Invoke());

            editProfileCommand = new RelayCommand(p => navigateToEdit?.Invoke());

            username = SessionService.username ?? "Loading...";
            welcomeMessage = string.Format("{0} {1}!", Lang.ProfileLbHi.TrimEnd('!'), username.ToUpper());
            avatarSource = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            achievements = new ObservableCollection<AchievementDto>();

            loadProfileData();
        }

        private async void loadProfileData()
        {
            if (string.IsNullOrEmpty(SessionService.username)) return;

            try
            {
                var client = new ProfileManagerClient();
                PlayerProfileViewDto profileData = await client.getPlayerProfileViewAsync(SessionService.username);

                if (profileData != null)
                {
                    // --- Map all data from DTO to ViewModel properties ---
                    username = profileData.username;
                    welcomeMessage = string.Format("{0} {1}!", Lang.ProfileLbHi.TrimEnd('!'), profileData.username.ToUpper());
                    avatarSource = profileData.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

                    // --- NEW FIELDS MAPPED ---
                    firstName = profileData.firstName;
                    lastName = profileData.lastName;
                    dateOfBirth = profileData.dateOfBirth?.ToString("dd/MM/yyyy") ?? "Not specified";
                    gender = profileData.gender ?? "Not specified";

                    // --- Stats Mapping ---
                    if (profileData.stats != null)
                    {
                        puzzlesCompleted = profileData.stats.puzzlesCompleted;
                        puzzlesWon = profileData.stats.puzzlesWon;
                        totalPlaytime = $"{profileData.stats.totalPlaytime.Hours}H {profileData.stats.totalPlaytime.Minutes}m";
                        highestScore = profileData.stats.highestScore;
                    }

                    // --- Achievements Mapping ---
                    if (profileData.achievements != null)
                    {
                        achievements = new ObservableCollection<AchievementDto>(profileData.achievements);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Lang.ErrorFailedToLoadProfile}: {ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}