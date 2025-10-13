using MindWeaveClient.ProfileService; // Asegúrate que este sea el namespace de tu referencia de servicio
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
        private string welcomeMessageValue;
        private string usernameValue;
        private string avatarSourceValue;
        private int puzzlesCompletedValue;
        private int puzzlesWonValue;
        private string totalPlaytimeValue;
        private int highestScoreValue;
        private ObservableCollection<AchievementDto> achievementsValue;

        public string welcomeMessage { get => welcomeMessageValue; set { welcomeMessageValue = value; OnPropertyChanged(); } }
        public string username { get => usernameValue; set { usernameValue = value; OnPropertyChanged(); } }
        public string avatarSource { get => avatarSourceValue; set { avatarSourceValue = value; OnPropertyChanged(); } }
        public int puzzlesCompleted { get => puzzlesCompletedValue; set { puzzlesCompletedValue = value; OnPropertyChanged(); } }
        public int puzzlesWon { get => puzzlesWonValue; set { puzzlesWonValue = value; OnPropertyChanged(); } }
        public string totalPlaytime { get => totalPlaytimeValue; set { totalPlaytimeValue = value; OnPropertyChanged(); } }
        public int highestScore { get => highestScoreValue; set { highestScoreValue = value; OnPropertyChanged(); } }
        public ObservableCollection<AchievementDto> achievements { get => achievementsValue; set { achievementsValue = value; OnPropertyChanged(); } }

        public ICommand backCommand { get; }
        public ICommand editProfileCommand { get; }

        public ProfileViewModel(Action navigateBack)
        {
            this.navigateBack = navigateBack;
            backCommand = new RelayCommand(p => navigateBack?.Invoke());
            // TO-DO: Implement Edit Profile Navigation
            editProfileCommand = new RelayCommand(p => MessageBox.Show("Edit profile not implemented yet."));

            // --- CORRECCIÓN ---
            // Inicializar propiedades con valores predeterminados para evitar errores de binding.
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
                    welcomeMessage = string.Format("{0} {1}!", Lang.ProfileLbHi.TrimEnd('!'), profileData.username.ToUpper());
                    username = profileData.username;
                    avatarSource = profileData.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

                    puzzlesCompleted = profileData.stats.puzzlesCompleted;
                    puzzlesWon = profileData.stats.puzzlesWon;
                    totalPlaytime = $"{profileData.stats.totalPlaytime.Hours}H {profileData.stats.totalPlaytime.Minutes}m";
                    highestScore = profileData.stats.highestScore;

                    achievements = new ObservableCollection<AchievementDto>(profileData.achievements);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load profile data: {ex.Message}", "Error");
            }
        }
    }
}