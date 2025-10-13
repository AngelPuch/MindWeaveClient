using MindWeaveClient.Services;
using MindWeaveClient.View.Main;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace MindWeaveClient.ViewModel.Main
{
    public class MainMenuViewModel : BaseViewModel
    {
        private readonly Action<Page> _navigateTo;
        private readonly Page _mainMenuPage;

        // --- Propiedades públicas en camelCase ---
        public string playerUsername { get; }
        public string playerAvatarPath { get; }

        // --- Comandos públicos en camelCase ---
        public ICommand profileCommand { get; }
        public ICommand playCommand { get; }
        public ICommand socialCommand { get; }
        public ICommand settingsCommand { get; }

        public MainMenuViewModel(Action<Page> navigateTo, Page mainMenuPage)
        {
            _navigateTo = navigateTo;
            _mainMenuPage = mainMenuPage;

            playerUsername = SessionService.username;
            playerAvatarPath = SessionService.avatarPath ?? "/Resources/Images/Avatar/default_avatar.png";

            profileCommand = new RelayCommand(p => executeGoToProfile());
            // TO-DO: Implementar otros comandos
        }

        // --- Métodos privados en camelCase ---
        private void executeGoToProfile()
        {
            var profilePage = new ProfilePage();
            profilePage.DataContext = new ProfileViewModel(
                () => _navigateTo(_mainMenuPage),
                () => executeGoToEditProfile()
            );
            _navigateTo(profilePage);
        }

        private void executeGoToEditProfile()
        {
            var editProfilePage = new EditProfilePage();
            editProfilePage.DataContext = new EditProfileViewModel(
                () => executeGoToProfile()
            );
            _navigateTo(editProfilePage);
        }
    }
}