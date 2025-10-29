using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class Avatar : BaseViewModel
    {
        private bool isSelectedValue;
        public string imagePath { get; set; }
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

        public ObservableCollection<Avatar> availableAvatars { get => availableAvatarsValue; set { availableAvatarsValue = value; OnPropertyChanged(); } }
        public Avatar selectedAvatar { get => selectedAvatarValue; set { selectedAvatarValue = value; OnPropertyChanged(); ((RelayCommand)saveSelectionCommand).RaiseCanExecuteChanged(); } } // Update CanExecute when selected changes
        public bool isBusy { get => isBusyValue; private set { SetBusy(value); } } 

        public ICommand saveSelectionCommand { get; }
        public ICommand cancelCommand { get; }

        public SelectAvatarViewModel(Action navigateBack)
        {
            this.navigateBack = navigateBack;
            cancelCommand = new RelayCommand(p => this.navigateBack?.Invoke(), p => !isBusy);
            saveSelectionCommand = new RelayCommand(async p => await saveSelection(), p => canSave());

            loadAvailableAvatars();
        }

        private void loadAvailableAvatars()
        {
            availableAvatars = new ObservableCollection<Avatar>();
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
                availableAvatars.Add(new Avatar { imagePath = path });
            }

            var currentAvatar = availableAvatars.FirstOrDefault(a => a.imagePath.Equals(SessionService.avatarPath, StringComparison.OrdinalIgnoreCase));
            if (currentAvatar != null)
            {
                currentAvatar.IsSelected = true;
                selectedAvatar = currentAvatar;
            }
        }

        private bool canSave() => selectedAvatar != null && !isBusy;


        private async Task saveSelection()
        {
            if (!canSave()) return;

            SetBusy(true);
            try
            {
                var client = new ProfileManagerClient();
                var result = await client.updateAvatarPathAsync(SessionService.username, selectedAvatar.imagePath);

                if (result.success)
                {
                    SessionService.updateAvatarPath(selectedAvatar.imagePath);
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
                SetBusy(false); 
            }
        }

        private void SetBusy(bool value)
        {
            isBusyValue = value;
            OnPropertyChanged(nameof(isBusy));
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }
    }
}