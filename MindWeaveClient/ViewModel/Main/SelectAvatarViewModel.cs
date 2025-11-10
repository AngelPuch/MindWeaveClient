using MindWeaveClient.ProfileService;
using MindWeaveClient.Properties.Langs;
using MindWeaveClient.Services;
using MindWeaveClient.Services.Abstractions;
using MindWeaveClient.Utilities.Abstractions;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class AvatarData
    {
        /// <summary>
        /// Gets or sets the resource path to the avatar image.
        /// </summary>
        public string ImagePath { get; set; }
    }
    /// <summary>
    /// ViewModel for the SelectAvatarPage.
    /// Manages loading the list of available avatars and saving the user's selection.
    /// </summary>
    public class SelectAvatarViewModel : BaseViewModel
    {
        // Constants
        private static readonly string[] AVATAR_PATHS = new string[]
        {
            "/Resources/Images/Avatar/default_avatar.png",
            "/Resources/Images/Avatar/alien_avatar.png",
            "/Resources/Images/Avatar/goblin_avatar.png",
            "/Resources/Images/Avatar/ball_avatar.png",
            "/Resources/Images/Avatar/pirat_avatar.png",
            "/Resources/Images/Avatar/robot_avatar.png",
        };

        // Services
        private readonly INavigationService _navigationService;
        private readonly IProfileService _profileService;
        private readonly IDialogService _dialogService;

        // Backing Fields
        private ObservableCollection<AvatarData> _availableAvatarsValue;
        private AvatarData _selectedAvatarValue;
        private bool _isBusyValue;

        #region Public Properties

        /// <summary>
        /// Gets the collection of available avatars to display.
        /// </summary>
        public ObservableCollection<AvatarData> AvailableAvatars
        {
            get => _availableAvatarsValue;
            set { _availableAvatarsValue = value; OnPropertyChanged(); }
        }

        /// <summary>
        /// Gets or sets the currently selected avatar.
        /// This is bound to the ListBox's SelectedItem.
        /// </summary>
        public AvatarData SelectedAvatar
        {
            get => _selectedAvatarValue;
            set
            {
                _selectedAvatarValue = value;
                OnPropertyChanged();
                raiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets a value indicating whether a long-running operation is in progress.
        /// </summary>
        public bool IsBusy
        {
            get => _isBusyValue;
            private set { setBusy(value); }
        }

        /// <summary>
        /// Gets a value indicating whether the current selection can be saved.
        /// </summary>
        public bool CanSave => SelectedAvatar != null && !IsBusy;

        #endregion

        #region Commands

        /// <summary>
        /// Command to save the currently selected avatar.
        /// </summary>
        public ICommand SaveSelectionCommand { get; }

        /// <summary>
        /// Command to cancel selection and navigate back.
        /// </summary>
        public ICommand CancelCommand { get; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectAvatarViewModel"/> class.
        /// </summary>
        public SelectAvatarViewModel(
            INavigationService navigationService,
            IProfileService profileService,
            IDialogService dialogService)
        {
            _navigationService = navigationService;
            _profileService = profileService;
            _dialogService = dialogService;

            CancelCommand = new RelayCommand(p => _navigationService.goBack(), p => !IsBusy);
            SaveSelectionCommand = new RelayCommand(async p => await saveSelectionAsync(), p => CanSave);

            loadAvailableAvatars();
        }

        private void loadAvailableAvatars()
        {
            AvailableAvatars = new ObservableCollection<AvatarData>();

            foreach (var path in AVATAR_PATHS)
            {
                AvailableAvatars.Add(new AvatarData { ImagePath = path });
            }

            // Set the initial selection based on the session
            var currentAvatar = AvailableAvatars.FirstOrDefault(a => a.ImagePath.Equals(SessionService.AvatarPath, StringComparison.OrdinalIgnoreCase));
            if (currentAvatar != null)
            {
                // Setting SelectedAvatar will automatically update the ListBox's selection
                SelectedAvatar = currentAvatar;
            }
        }

        private async Task saveSelectionAsync()
        {
            if (!CanSave) return;

            setBusy(true);
            try
            {
                // Use the injected ProfileService, not a direct proxy client
                var result = await _profileService.updateAvatarPathAsync(SessionService.Username, SelectedAvatar.ImagePath);

                if (result.success)
                {
                    SessionService.UpdateAvatarPath(SelectedAvatar.ImagePath);
                    _dialogService.showInfo(result.message, Lang.InfoMsgTitleSuccess);
                    _navigationService.goBack();
                }
                else
                {
                    _dialogService.showError(result.message, Lang.ErrorTitle);
                }
            }
            catch (Exception ex)
            {
                handleError(Lang.ProfileUpdateError, ex); // Use a consistent Lang key
            }
            finally
            {
                setBusy(false);
            }
        }

        private void setBusy(bool value)
        {
            _isBusyValue = value;
            OnPropertyChanged(nameof(IsBusy));
            raiseCanExecuteChanged();
        }

        private void raiseCanExecuteChanged()
        {
            OnPropertyChanged(nameof(CanSave));
            Application.Current?.Dispatcher?.Invoke(() => CommandManager.InvalidateRequerySuggested());
        }

        private void handleError(string message, Exception ex)
        {
            Trace.TraceError($"Error in {nameof(SelectAvatarViewModel)}: {message} | Exception: {ex}");
            _dialogService.showError(message, Lang.ErrorTitle);
        }
    }
}