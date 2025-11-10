using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class AvatarData
    {
        public string ImagePath { get; set; }
    }

    public class SelectAvatarViewModel : BaseViewModel
    {
        private static readonly string[] avatarPaths = new string[]
        {
            "/Resources/Images/Avatar/default_avatar.png",
            "/Resources/Images/Avatar/alien_avatar.png",
            "/Resources/Images/Avatar/goblin_avatar.png",
            "/Resources/Images/Avatar/ball_avatar.png",
            "/Resources/Images/Avatar/pirat_avatar.png",
            "/Resources/Images/Avatar/robot_avatar.png",
        };

        private readonly INavigationService navigationService;
        private readonly IProfileService profileService;
        private readonly IDialogService dialogService;

        private ObservableCollection<AvatarData> availableAvatarsValue;
        private AvatarData selectedAvatarValue;

        public ObservableCollection<AvatarData> AvailableAvatars
        {
            get => availableAvatarsValue;
            set { availableAvatarsValue = value; OnPropertyChanged(); }
        }

        public AvatarData SelectedAvatar
        {
            get => selectedAvatarValue;
            set
            {
                selectedAvatarValue = value;
                OnPropertyChanged();
                raiseCanExecuteChanged();
            }
        }

        public bool CanSave => SelectedAvatar != null && !IsBusy;

        public ICommand SaveSelectionCommand { get; }

        public ICommand CancelCommand { get; }

        public SelectAvatarViewModel(
            INavigationService navigationService,
            IProfileService profileService,
            IDialogService dialogService)
        {
            this.navigationService = navigationService;
            this.profileService = profileService;
            this.dialogService = dialogService;

            CancelCommand = new RelayCommand(p => this.navigationService.goBack(), p => !IsBusy);
            SaveSelectionCommand = new RelayCommand(async p => await saveSelectionAsync(), p => CanSave);

            loadAvailableAvatars();
        }

        private void loadAvailableAvatars()
        {
            AvailableAvatars = new ObservableCollection<AvatarData>();

            foreach (var path in avatarPaths)
            {
                AvailableAvatars.Add(new AvatarData { ImagePath = path });
            }

            var currentAvatar = AvailableAvatars.FirstOrDefault(a => a.ImagePath.Equals(SessionService.AvatarPath, StringComparison.OrdinalIgnoreCase));
            if (currentAvatar != null)
            {
                SelectedAvatar = currentAvatar;
            }
        }

        private async Task saveSelectionAsync()
        {
            if (!CanSave) return;

            setBusy(true);
            try
            {
                var result = await profileService.updateAvatarPathAsync(SessionService.Username, SelectedAvatar.ImagePath);

                if (result.success)
                {
                    SessionService.updateAvatarPath(SelectedAvatar.ImagePath);
                    dialogService.showInfo(result.message, Lang.InfoMsgTitleSuccess);
                    navigationService.goBack();
                }
                else
                {
                    dialogService.showError(result.message, Lang.ErrorTitle);
                }
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

        private void raiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(CanSave));
        }

        private void handleError(string message, Exception ex)
        {
            dialogService.showError(message, Lang.ErrorTitle);
        }
    }
}