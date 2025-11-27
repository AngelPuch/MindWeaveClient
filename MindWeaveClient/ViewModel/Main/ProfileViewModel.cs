using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using MindWeaveClient.View.Main;
using System;
using System.Collections.ObjectModel;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Input;

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
        public ObservableCollection<AchievementDto> AchievementsList { get; set; }

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
            AchievementsList = new ObservableCollection<AchievementDto>();
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

            _ = loadProfileDataAsync();
            LoadAchievements();
        }

        private void OnAvatarPathChanged(object sender, EventArgs e)
        {
            AvatarSource = SessionService.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";
        }

        ~ProfileViewModel()
        {
            SessionService.AvatarPathChanged -= OnAvatarPathChanged;
        }


        private async Task loadProfileDataAsync()
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
                    Username = profileData.Username;
                    WelcomeMessage = $"{Lang.ProfileLbHi.TrimEnd('!')} {profileData.Username.ToUpper()}!";
                    AvatarSource = profileData.AvatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

                    FirstName = profileData.FirstName ?? Lang.GlobalLbNotSpecified;
                    LastName = profileData.LastName ?? Lang.GlobalLbNotSpecified;
                    DateOfBirth = profileData.DateOfBirth?.ToString("dd/MM/yyyy") ?? Lang.GlobalLbNotSpecified;
                    Gender = profileData.Gender ?? Lang.GlobalLbNotSpecified;

                    if (profileData.Stats != null)
                    {
                        PuzzlesCompleted = profileData.Stats.PuzzlesCompleted;
                        PuzzlesWon = profileData.Stats.PuzzlesWon;
                        TotalPlaytime = $"{profileData.Stats.TotalPlaytime.Hours}H {profileData.Stats.TotalPlaytime.Minutes}m";
                        HighestScore = profileData.Stats.HighestScore;
                    }

                    if (profileData.Achievements != null)
                    {
                        Achievements = new ObservableCollection<AchievementDto>(profileData.Achievements);
                    }
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
                handleError(Lang.ErrorFailedToLoadProfile, ex);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void handleError(string message, Exception ex)
        {
            string errorDetails = ex != null ? ex.Message : Lang.ErrorMsgNoDetails;
            dialogService.showError($"{message}\n{Lang.ErrorTitleDetails}: {errorDetails}", Lang.ErrorTitle);
        }


        private async void LoadAchievements()
        {
            if (SessionService.PlayerId <= 0)
            {
                return;
            }

            int currentPlayerId = SessionService.PlayerId;
            ProfileManagerClient client = new ProfileManagerClient();

            try
            {
                var achievements = await client.GetPlayerAchievementsAsync(currentPlayerId);

                AchievementsList.Clear();
                foreach (var achievement in achievements)
                {

                    string fileName = System.IO.Path.GetFileName(achievement.IconPath);
                    achievement.IconPath = $"pack://application:,,,/MindWeaveClient;component/Resources/Images/achievements/{fileName}";
                    AchievementsList.Add(achievement);
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
                handleError(Lang.ErrorFailedToLoadProfile, ex);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                try
                {
                    if (client.State == CommunicationState.Opened)
                        client.Close();
                    else
                        client.Abort();
                }
                catch
                {
                    client.Abort();
                }
            }
        }
    }
}