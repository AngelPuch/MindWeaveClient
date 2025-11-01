using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly Action navigateBack;
        private readonly Action navigateToEdit;

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

        public string WelcomeMessage { get => welcomeMessageValue; set { welcomeMessageValue = value; OnPropertyChanged(); } }
        public string Username { get => usernameValue; set { usernameValue = value; OnPropertyChanged(); } }
        public string AvatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }
        public string FirstName { get => firstNameValue; set { firstNameValue = value; OnPropertyChanged(); } }
        public string LastName { get => lastNameValue; set { lastNameValue = value; OnPropertyChanged(); } }
        public string DateOfBirth { get => dateOfBirthValue; set { dateOfBirthValue = value; OnPropertyChanged(); } }
        public string Gender { get => genderValue; set { genderValue = value; OnPropertyChanged(); } }
        public int PuzzlesCompleted { get => puzzlesCompletedValue; set { puzzlesCompletedValue = value; OnPropertyChanged(); } }
        public int PuzzlesWon { get => puzzlesWonValue; set { puzzlesWonValue = value; OnPropertyChanged(); } }
        public string TotalPlaytime { get => totalPlaytimeValue; set { totalPlaytimeValue = value; OnPropertyChanged(); } }
        public int HighestScore { get => highestScoreValue; set { highestScoreValue = value; OnPropertyChanged(); } }
        public ObservableCollection<AchievementDto> Achievements { get => achievementsValue; set { achievementsValue = value; OnPropertyChanged(); } }

        public ICommand BackCommand { get; }
        public ICommand EditProfileCommand { get; }

        public ProfileViewModel(Action navigateBack, Action navigateToEdit)
        {
            this.navigateBack = navigateBack;
            this.navigateToEdit = navigateToEdit;

            BackCommand = new RelayCommand(p => navigateBack?.Invoke());

            EditProfileCommand = new RelayCommand(p => navigateToEdit?.Invoke());

            Username = SessionService.Username ?? "Loading...";
            WelcomeMessage = string.Format("{0} {1}!", Lang.ProfileLbHi.TrimEnd('!'), Username.ToUpper());
            AvatarSource = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            Achievements = new ObservableCollection<AchievementDto>();

            loadProfileData();
        }

        private async void loadProfileData()
        {
            if (string.IsNullOrEmpty(SessionService.Username)) return;

            try
            {
                var client = new ProfileManagerClient();
                PlayerProfileViewDto profileData = await client.getPlayerProfileViewAsync(SessionService.Username);

                if (profileData != null)
                {
                    Username = profileData.username;
                    WelcomeMessage = string.Format("{0} {1}!", Lang.ProfileLbHi.TrimEnd('!'), profileData.username.ToUpper());
                    AvatarSource = profileData.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

                    FirstName = profileData.firstName;
                    LastName = profileData.lastName;
                    DateOfBirth = profileData.dateOfBirth?.ToString("dd/MM/yyyy") ?? "Not specified";
                    Gender = profileData.gender ?? "Not specified";

                    if (profileData.stats != null)
                    {
                        PuzzlesCompleted = profileData.stats.puzzlesCompleted;
                        PuzzlesWon = profileData.stats.puzzlesWon;
                        TotalPlaytime = $"{profileData.stats.totalPlaytime.Hours}H {profileData.stats.totalPlaytime.Minutes}m";
                        HighestScore = profileData.stats.highestScore;
                    }

                    if (profileData.achievements != null)
                    {
                        Achievements = new ObservableCollection<AchievementDto>(profileData.achievements);
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