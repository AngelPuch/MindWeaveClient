using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MindWeaveClient.View.Main;

namespace MindWeaveClient.ViewModel.Main
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly IProfileService profileService;
        private readonly IDialogService dialogService;

        private string welcomeMessage;
        private string username;
        private string avatarSource;
        private string firstName;
        private string lastName;
        private string dateOfBirth;
        private string gender;
        private int puzzlesCompleted;
        private int puzzlesWon;
        private string totalPlaytime;
        private int highestScore;
        private ObservableCollection<AchievementDto> achievements;

        public string WelcomeMessage { get => welcomeMessage; set { welcomeMessage = value; OnPropertyChanged(); } }
        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        public string AvatarSource { get => avatarSource; set { avatarSource = value; OnPropertyChanged(); } }
        public string FirstName { get => firstName; set { firstName = value; OnPropertyChanged(); } }
        public string LastName { get => lastName; set { lastName = value; OnPropertyChanged(); } }
        public string DateOfBirth { get => dateOfBirth; set { dateOfBirth = value; OnPropertyChanged(); } }
        public string Gender { get => gender; set { gender = value; OnPropertyChanged(); } }
        public int PuzzlesCompleted { get => puzzlesCompleted; set { puzzlesCompleted = value; OnPropertyChanged(); } }
        public int PuzzlesWon { get => puzzlesWon; set { puzzlesWon = value; OnPropertyChanged(); } }
        public string TotalPlaytime { get => totalPlaytime; set { totalPlaytime = value; OnPropertyChanged(); } }
        public int HighestScore { get => highestScore; set { highestScore = value; OnPropertyChanged(); } }
        public ObservableCollection<AchievementDto> Achievements { get => achievements; set { achievements = value; OnPropertyChanged(); } }

        public ICommand BackCommand { get; }
        public ICommand EditProfileCommand { get; }

        public ProfileViewModel(
            IProfileService profileService,
            IDialogService dialogService,
            INavigationService navigationService)
        {
            this.profileService = profileService;
            this.dialogService = dialogService;
            var navigationService1 = navigationService;

            SessionService.AvatarPathChanged += OnAvatarPathChanged;

            BackCommand = new RelayCommand(p => navigationService1.goBack(), p => !IsBusy);
            EditProfileCommand = new RelayCommand(p => navigationService1.navigateTo<EditProfilePage>(), p => !IsBusy);

            Username = SessionService.Username ?? Lang.GlobalLbLoading;
            WelcomeMessage = $"{Lang.ProfileLbHi.TrimEnd('!')} {Username.ToUpper()}!";
            AvatarSource = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
            Achievements = new ObservableCollection<AchievementDto>();

            loadProfileDataAsync();
        }

        private void OnAvatarPathChanged(object sender, EventArgs e)
        {
            AvatarSource = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        ~ProfileViewModel()
        {
            SessionService.AvatarPathChanged -= OnAvatarPathChanged;
        }


        private async void loadProfileDataAsync()
        {
            if (string.IsNullOrEmpty(SessionService.Username))
            {
                return;
            }

            SetBusy(true);
            try
            {
                PlayerProfileViewDto profileData = await profileService.getPlayerProfileViewAsync(SessionService.Username);

                if (profileData != null)
                {
                    Username = profileData.username;
                    WelcomeMessage = $"{Lang.ProfileLbHi.TrimEnd('!')} {profileData.username.ToUpper()}!";
                    AvatarSource = profileData.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

                    FirstName = profileData.firstName ?? Lang.GlobalLbNotSpecified;
                    LastName = profileData.lastName ?? Lang.GlobalLbNotSpecified;
                    DateOfBirth = profileData.dateOfBirth?.ToString("dd/MM/yyyy") ?? Lang.GlobalLbNotSpecified;
                    Gender = profileData.gender ?? Lang.GlobalLbNotSpecified;

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
                dialogService.showError(Lang.ErrorFailedToLoadProfile + ex.Message, Lang.ErrorTitle);
            }
            finally
            {
                SetBusy(false);
            }
        }
    }
}