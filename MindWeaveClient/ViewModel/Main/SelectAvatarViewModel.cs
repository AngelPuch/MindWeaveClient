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
    public class Avatar : BaseViewModel
    {
        private bool isSelectedValue;
        public string ImagePath { get; set; }
        public bool IsSelected
        {
            get => isSelectedValue;
            set { isSelectedValue = value; OnPropertyChanged(); }
        }
    }

    public class SelectAvatarViewModel : BaseViewModel
    {
        private readonly Action navigateBack;
        private ObservableCollection<Avatar> availableAvatarsValue;
        private Avatar selectedAvatarValue;
        private bool isBusyValue;

        public ObservableCollection<Avatar> AvailableAvatars { get => availableAvatarsValue; set { availableAvatarsValue = value; OnPropertyChanged(); } }
        public Avatar SelectedAvatar { get => selectedAvatarValue; set { selectedAvatarValue = value; OnPropertyChanged(); ((RelayCommand)SaveSelectionCommand).RaiseCanExecuteChanged(); } } // Update CanExecute when selected changes
        public bool IsBusy { get => isBusyValue; private set { setBusy(value); } } 

        public ICommand SaveSelectionCommand { get; }
        public ICommand CancelCommand { get; }

        public SelectAvatarViewModel(Action navigateBack)
        {
            this.navigateBack = navigateBack;
            CancelCommand = new RelayCommand(p => this.navigateBack?.Invoke(), p => !IsBusy);
            SaveSelectionCommand = new RelayCommand(async p => await saveSelection(), p => canSave());

            loadAvailableAvatars();
        }

        private void loadAvailableAvatars()
        {
            AvailableAvatars = new ObservableCollection<Avatar>();
            var avatarPaths = new string[]
            {
                "/Resources/Images/Avatar/default_avatar.png",
                "/Resources/Images/Avatar/alien_avatar.png",
                "/Resources/Images/Avatar/goblin_avatar.png",
                "/Resources/Images/Avatar/ball_avatar.png",
                "/Resources/Images/Avatar/pirat_avatar.png",
                "/Resources/Images/Avatar/robot_avatar.png",
            };

            foreach (var path in avatarPaths)
            {
                AvailableAvatars.Add(new Avatar { ImagePath = path });
            }

            var currentAvatar = AvailableAvatars.FirstOrDefault(a => a.ImagePath.Equals(SessionService.AvatarPath, StringComparison.OrdinalIgnoreCase));
            if (currentAvatar != null)
            {
                currentAvatar.IsSelected = true;
                SelectedAvatar = currentAvatar;
            }
        }

        private bool canSave() => SelectedAvatar != null && !IsBusy;


        private async Task saveSelection()
        {
            if (!canSave()) return;

            setBusy(true);
            try
            {
                var client = new ProfileManagerClient();
                var result = await client.updateAvatarPathAsync(SessionService.Username, SelectedAvatar.ImagePath);

                if (result.success)
                {
                    SessionService.UpdateAvatarPath(SelectedAvatar.ImagePath);
                    MessageBox.Show(result.message, Lang.InfoMsgTitleSuccess, MessageBoxButton.OK, MessageBoxImage.Information); // Use Lang key
                    navigateBack?.Invoke();
                }
                else
                {
                    MessageBox.Show(result.message, Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ex.Message}", Lang.ErrorTitle, MessageBoxButton.OK, MessageBoxImage.Error); // Use Lang key
            }
            finally
            {
                setBusy(false); 
            }
        }

        private void setBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(IsBusy));
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }
    }
}